using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;

public class HeightBody2D : MonoBehaviour
{
    public static float tileSize = 1f;
    
    [ColorHeader("Dependencies")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private BoxCollider2D physicsBox;

    [ColorHeader("Collision Config", ColorHeaderColor.Config)]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private LayerMask hazardMask;
    
    [SerializeField] private float colliderHeight = 1;
    [SerializeField] private float collisionOffset = 0.001f;
    [SerializeField] private float skinWidth = 0.03f;
    [SerializeField] private int maxResolves = 10;
    [SerializeField] private float snapToStairsMaxYVel = 4f;

    [ColorHeader("Shadow", ColorHeaderColor.Config)]
    [SerializeField] private Transform shadowTransform;
    [SerializeField] private float maxShadowCastDist;
    private bool hasShadow;

    public Transform ShadowTransform => shadowTransform;
    
    [ColorHeader("Physics State")]
    public Vector2 horizontalPos;
    public Vector2 horizontalVel;
    
    public float height;
    public float verticalVelocity;
    public float gravityAccel = 13;
    public float terminalVelocity = 15;
    public bool gravityEnabled = true;
    public bool horizontalCollideEnabled = true;

    public bool isGrounded;
    private float xTilt;
    private float nearestGroundIfNotGrounded = -1;

    private Vector2 expectedWallHorizontalPos;

    public Vector3 TransformPosition => targetTransform.position;
    
    // Events
    public event Action<Vector2, Vector2> onHorizontalCollide;
    public event Action onHitGround;
    public event Action onHitHazard;
    
    private void OnEnable()
    {
        hasShadow = shadowTransform != null;
    }

    public void ForceUpdate()
    {
        FixedUpdate();
        RecalculatePosition(true);
    }

    private void FixedUpdate()
    {
        Vector2 startHorizontalCoords = horizontalPos;
        ApplyForces();

        bool wasGrounded = isGrounded;
        VerticalCollisions();

        // Prevent falling in straight directions to prevent clipping
        if (!isGrounded && wasGrounded)
        {
            horizontalVel = horizontalVel.normalized * Mathf.Max(horizontalVel.magnitude, 3f);
        }
        else if (isGrounded && !wasGrounded)
        {
            onHitGround?.Invoke();
        }
        
        Vector2 horizontalStep = horizontalVel  * Time.fixedDeltaTime;
        horizontalStep.y *= Mathf.Cos(xTilt);
        if (horizontalCollideEnabled)
        {
            Vector2 startStep = horizontalStep;
            Vector2 startVel = horizontalVel;
            
            HorizontalCollisions(ref horizontalStep, 0);
            
            Vector2 impactVel = (startStep - horizontalStep) / Time.fixedDeltaTime;
            if (impactVel != Vector2.zero)
            {
                onHorizontalCollide?.Invoke(impactVel, startVel);
            }
        }

        ApplyVelocity(horizontalStep, startHorizontalCoords);
        RecalculatePosition();

        if (height < -14f)
        {
            onHitHazard?.Invoke();
        }
    }
    

#if UNITY_EDITOR
    private void OnValidate()
    {
        RecalculatePosition();
    }
#endif

    private void ApplyForces()
    {
        // Gravity and terminal velocity
        if (gravityEnabled)
        {
            verticalVelocity -= Time.fixedDeltaTime * gravityAccel;
            verticalVelocity = Mathf.Max(verticalVelocity, -terminalVelocity);
        }
    }

    private void HorizontalCollisions(ref Vector2 step, int bounces)
    {
        if (bounces > maxResolves)
            return;
        Vector2 dir = step.normalized;
        float stepDist = step.magnitude;
        RecalculatePosition();
        
        var hits = Physics2D.BoxCastAll(
            physicsBox.bounds.center,
            physicsBox.size,
            0f,
            step,
            stepDist + skinWidth,
            wallMask
        );

        float thisLowerBound = height + 0.01f;
        float thisUpperBound  = height + colliderHeight;

        RaycastHit2D closest = default;

        float closestDist = float.MaxValue;
        foreach(var hit in hits)
        {
            var hitTransform = hit.collider.transform;
            float baseHeight = hitTransform.position.z;
            float scale = hitTransform.localScale.z;

            float colliderUpper = baseHeight + scale;
            float colliderLower = baseHeight;
            
            // Try to approximate where the collision took place on the horizontal coordinate
            Vector2 horizontalHitPosition = hit.point;
            
            if (Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.y))
            {
                // Hit a wall from the side (x direction)
                // In this case the hit Y is relevant for horizontal position
                horizontalHitPosition.y -= baseHeight;
            }
            else
            {
                // Hit a wall from the up/down (y direction)
                // In this case the hit Y is irrelevant for horizontal position, so we assume all wall colliders have heights of one
                horizontalHitPosition.y = (int)horizontalHitPosition.y;
                horizontalHitPosition.y -= baseHeight;
            }

            // If the character is moving away from the collider on the horizontal axis, then ignore
            // This prevents collisions when dropping down height through a wall collider
            Vector2 horizontalNormal = horizontalPos - horizontalHitPosition;
            float directionDot = Vector2.Dot(horizontalNormal, step);

            bool fallingCollision = isGrounded || directionDot < 0f;

            bool intersectionA = thisUpperBound >= colliderLower && thisUpperBound <= colliderUpper;
            bool intersectionB = thisLowerBound >= colliderLower && thisLowerBound <= colliderUpper;
            expectedWallHorizontalPos = horizontalHitPosition;
            if (fallingCollision && (intersectionA || intersectionB))
            {

                if (hit.distance < closestDist)
                {
                    closest = hit;
                    closestDist = hit.distance;
                }
            }
        }

        if (closest != default)
        {

            float hitDist = closest.distance - skinWidth;
            
            Vector2 target = horizontalPos + step;
            // Snap to the collision surface
            horizontalPos += dir * hitDist;
            
            step = target - horizontalPos;
            
            // "Slide" up the surface
            step = step.ProjectOntoNormal(closest.normal.normalized);
            
            horizontalVel = horizontalVel.ProjectOntoNormal(closest.normal);
            //horizontalVel += hit.normal * collisionOffset;
            step += closest.normal * collisionOffset;
            
            // Recursively resolve
            HorizontalCollisions(ref step, bounces + 1);
        }
    }

    private void ShadowCast()
    {
        
    }

    private void VerticalCollisions()
    {
        Vector2 testPoint = physicsBox.bounds.center;
        float heightStep = verticalVelocity * Time.fixedDeltaTime;
        
        (float,float) CalculateGroundHeight(Collider2D coll)
        {
            var colliderPos = coll.bounds.center;
            float groundHeight = colliderPos.z;

            var ramp = coll.GetComponent<VerticalHeightRamp>();
            bool isRamp = ramp != null;
            
            float groundXTilt = 0f;
            if (isRamp)
            {
                groundXTilt = ramp.Angle;
                groundHeight = ramp.EvaluateHeight(horizontalPos);
            }

            return (groundHeight, groundXTilt);
        }

        isGrounded = false;
        xTilt = 0f;

        float closest = float.MinValue;
        var casts = Physics2D.RaycastAll(
            testPoint,
            Vector2.down,
            2 * heightStep,
            groundMask | hazardMask);
        
        foreach (var cast in casts)
        {
            // Resolve collisions
            var colliderPos = cast.collider.bounds.center;
            

            Vector2 collisionGroundPos = cast.point;
            collisionGroundPos.y -= colliderPos.z;

            var res = CalculateGroundHeight(cast.collider);

            float groundHeight = res.Item1;
            float groundXTilt = res.Item2;
            bool isRamp = groundXTilt != 0;

            bool isCloser = groundHeight > closest;
            bool isWithinNextVertStep = groundHeight >= height + heightStep;
            bool isWithinSnapUpDist = groundHeight <= height + 0.25f;

            bool fallDownRamp = horizontalVel.y <= -snapToStairsMaxYVel;
            bool snapToRamp = !fallDownRamp && isRamp && groundHeight >= height - 0.2f;

            Vector2 hitHorizontal = cast.point - Vector2.up * groundHeight;

            if (isCloser && (snapToRamp || isWithinNextVertStep) && isWithinSnapUpDist)
            {
                isGrounded = true;
                xTilt = groundXTilt;
                verticalVelocity = 0f;

                height = groundHeight;
                closest = groundHeight;
                
                // Hazard check
                if (((1 << cast.collider.gameObject.layer) | hazardMask) == hazardMask)
                {
                    onHitHazard?.Invoke();
                }
            }
        }

        
        if (!isGrounded)
        {
            // Check further for ground to place shadows correctly
            float castsPerUnit = 4;
            nearestGroundIfNotGrounded = -1;
            float nearestHorizontalDist = float.MaxValue;
        
            for (int i = 0; i < maxShadowCastDist * castsPerUnit; i++)
            {
                var cast = Physics2D.Raycast(
                    testPoint - Vector2.down * i/castsPerUnit,
                    Vector2.down,
                    1/castsPerUnit,
                    groundMask | hazardMask);
                if (cast.collider != null)
                {
                    var res = CalculateGroundHeight(cast.collider);
                    Vector2 hitHorizontal = cast.point - Vector2.up * res.Item1;

                    float horizontalDist = (horizontalPos - hitHorizontal).sqrMagnitude;
                    if(res.Item1 < height && horizontalDist < nearestHorizontalDist)
                    {
                        nearestHorizontalDist = horizontalDist;
                        nearestGroundIfNotGrounded = res.Item1;
                    }
                }
            }
        }
    }

    private void ApplyVelocity(Vector2 horizontalStep, Vector2 startHorizontalCoords)
    {
        horizontalPos += horizontalStep;
        
        Vector2 finalHorizontalStep = horizontalPos - startHorizontalCoords;

        if (isGrounded)
        {
            /*float vertical = finalHorizontalStep.y * Mathf.Tan(xTilt);
            height += vertical;*/
        }
        else
        {
            height += verticalVelocity * Time.fixedDeltaTime; 
        }
    }

    public void RecalculatePosition(bool forceSnapShadow = false)
    {
        transform.position = new Vector3(
            horizontalPos.x,
            horizontalPos.y + height,
            height);

        if (hasShadow)
        {
            float y = horizontalPos.y;
            if (isGrounded)
            {
                shadowTransform.gameObject.SetActive(true);
                y += height;
            }
            else if(nearestGroundIfNotGrounded != -1)
            {
                shadowTransform.gameObject.SetActive(true);
                y += nearestGroundIfNotGrounded;
            }
            else
            {
                shadowTransform.gameObject.SetActive(false);
            }
            
            float distance = horizontalPos.y + height - y;

            shadowTransform.position = Vector3.MoveTowards(
                shadowTransform.position,
                new Vector3(horizontalPos.x, y, height),
                forceSnapShadow ? 1000000 : 50f * Time.fixedDeltaTime
            );

            shadowTransform.localScale = Vector3.one * Mathf.InverseLerp(maxShadowCastDist, 0f, distance);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)physicsBox.bounds.center - Vector2.up * height, 0.3f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(expectedWallHorizontalPos, 0.3f);
    }
}
