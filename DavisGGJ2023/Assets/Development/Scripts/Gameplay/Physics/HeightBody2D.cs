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
    public Vector2 horizontalCoords;
    public Vector2 horizontalVel;
    
    public float height;
    public float verticalVelocity;
    public float gravityAccel = 13;
    public float terminalVelocity = 15;
    public bool gravityEnabled = true;

    public bool isGrounded;
    private float xTilt;
    private float nearestGroundIfNotGrounded;

    private Vector2 expectedWallHorizontalPos;

    public Vector2 TransformPosition => targetTransform.position;
    
    // Events
    public event Action<Vector2, Vector2> onHorizontalCollide;
    public event Action onHitHazard;
    
    private void OnEnable()
    {
        hasShadow = shadowTransform != null;
    }

    private void FixedUpdate()
    {
        Vector2 startHorizontalCoords = horizontalCoords;
        ApplyForces();

        bool wasGrounded = isGrounded;
        VerticalCollisions();

        // Prevent falling in straight directions to prevent clipping
        if (!isGrounded && wasGrounded)
        {
            horizontalVel = horizontalVel.normalized * Mathf.Max(horizontalVel.magnitude, 3f);
        }
        
        Vector2 horizontalStep = horizontalVel  * Time.fixedDeltaTime;
        horizontalStep.y *= Mathf.Cos(xTilt);
        // To prevent weird collisions when dropping down
        //if (isGrounded)
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
            Vector2 horizontalNormal = horizontalCoords - horizontalHitPosition;
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
            
            Vector2 target = horizontalCoords + step;
            // Snap to the collision surface
            horizontalCoords += dir * hitDist;
            
            step = target - horizontalCoords;
            
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
        // Check for ground
        var casts = Physics2D.RaycastAll(
            testPoint,
            Vector2.down,
            0.25f,
            groundMask | hazardMask);
        
        isGrounded = false;
        xTilt = 0f;

        float closest = float.MinValue;
        nearestGroundIfNotGrounded = -1;
        
        foreach (var cast in casts)
        {
            // Resolve collisions
            var colliderPos = cast.collider.bounds.center;
            float groundHeight = colliderPos.z;

            var ramp = cast.collider.GetComponent<VerticalHeightRamp>();
            bool isRamp = ramp != null;
            
            float groundXTilt = 0f;
            if (isRamp)
            {
                groundXTilt = ramp.Angle;
                groundHeight = ramp.EvaluateHeight(horizontalCoords);
            }

            Vector2 collisionGroundPos = cast.point;
            collisionGroundPos.y -= colliderPos.z;

            bool isCloser = groundHeight > closest;
            bool isWithinNextVertStep = groundHeight >= height + heightStep;
            bool isWithinSnapUpDist = groundHeight <= height + 0.25f;

            bool fallDownRamp = horizontalVel.y <= -snapToStairsMaxYVel;
            bool snapToRamp = !fallDownRamp && isRamp && groundHeight >= height - 0.2f;
            
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
            else
            {
                nearestGroundIfNotGrounded = groundHeight;
            }
        }
    }

    private void ApplyVelocity(Vector2 horizontalStep, Vector2 startHorizontalCoords)
    {
        horizontalCoords += horizontalStep;
        
        Vector2 finalHorizontalStep = horizontalCoords - startHorizontalCoords;

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

    private void RecalculatePosition()
    {
        transform.position = new Vector3(
            horizontalCoords.x,
            horizontalCoords.y + height,
            height);

        if (hasShadow)
        {
            float y = horizontalCoords.y;
            if (isGrounded)
            {
                y += height;
            }
            else if(nearestGroundIfNotGrounded != -1)
            {
                y += nearestGroundIfNotGrounded;
            }
            else
            {
                y = -10000f;
            }
            float distance = horizontalCoords.y + height - y;
            shadowTransform.position = new Vector3(
                horizontalCoords.x,
                y,
                height);

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
