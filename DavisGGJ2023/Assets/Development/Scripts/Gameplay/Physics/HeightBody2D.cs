using System;
using System.Collections.Generic;
using UnityEngine;

public class HeightBody2D : MonoBehaviour
{
    public static float tileSize = 1f;
    
    [ColorHeader("Dependencies")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private BoxCollider2D physicsBox;

    [ColorHeader("Collision Config", ColorHeaderColor.Config)]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float colliderHeight;
    [SerializeField] private float collisionOffset;
    [SerializeField] private float skinWidth;
    [SerializeField] private int maxResolves;

    [ColorHeader("Height Changers", ColorHeaderColor.Config)]
    [SerializeField] private LayerMask heightChangerMask;

    [ColorHeader("Shadow", ColorHeaderColor.Config)]
    [SerializeField] private Transform shadowTransform;
    [SerializeField] private float maxShadowCastDist;
    private bool hasShadow;
    
    [ColorHeader("Physics State")]
    public Vector2 horizontalCoords;
    public Vector2 horizontalVel;
    
    public float height;
    public float verticalVelocity;
    public float gravityAccel;
    public float terminalVelocity;

    public bool isGrounded;
    private float xTilt;
    private float nearestGroundIfNotGrounded;
    private void OnEnable()
    {
        hasShadow = shadowTransform != null;
    }

    private void FixedUpdate()
    {
        Vector2 startHorizontalCoords = horizontalCoords;
        ApplyForces();
        VerticalCollisions();
        
        Vector2 horizontalStep = horizontalVel  * Time.fixedDeltaTime;
        horizontalStep.y *= Mathf.Cos(xTilt);

        if(isGrounded) 
            HorizontalCollisions(ref horizontalStep, 0);
        
        ApplyVelocity(horizontalStep, startHorizontalCoords);
        RecalculatePosition();
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
        verticalVelocity -= Time.fixedDeltaTime * gravityAccel;
        verticalVelocity = Mathf.Max(verticalVelocity, -terminalVelocity);
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
            float colliderLower = baseHeight - scale;

            bool intersectionA = thisUpperBound >= colliderLower && thisUpperBound <= colliderUpper;
            bool intersectionB = thisLowerBound >= colliderLower && thisLowerBound <= colliderUpper;
            
            if (intersectionA || intersectionB)
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
        var casts = Physics2D.BoxCastAll(
            testPoint,
            physicsBox.size,
            0f,
            Vector2.down,
            Mathf.Abs(heightStep),
            groundMask);
        
        isGrounded = false;
        xTilt = 0f;
        
        float closest = float.MinValue;
        float y = horizontalCoords.y;
        nearestGroundIfNotGrounded = -1;
        foreach (var cast in casts)
        {
            var pos = cast.transform.position;
            float groundHeight = pos.z;

            var ramp = cast.collider.GetComponent<VerticalHeightRamp>();
            float groundXTilt = 0f;
            if (ramp != null)
            {
                groundXTilt = Mathf.Deg2Rad * ramp.Angle;
                groundHeight = ramp.EvaluateHeight(cast.point);
            }

            if (groundHeight > closest && groundHeight >= height + 2f * heightStep && groundHeight <= height + 0.5f)
            {
                isGrounded = true;
                
                // Offset Y to compensate for height snapping to allow for smooth movement
                xTilt = groundXTilt;
                if (xTilt == 0)
                {
                    y = horizontalCoords.y;
                }
                else
                {
                    float heightGained = groundHeight - height;
                    y = horizontalCoords.y - heightGained;
                    
                    // Snap to stairs when going down

                }
                verticalVelocity = 0f;
                height = groundHeight;
                closest = groundHeight;
            }
            else
            {
                nearestGroundIfNotGrounded = groundHeight;
            }
        }

        horizontalCoords.y = y;
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

            shadowTransform.localScale = Vector3.one * Mathf.InverseLerp(2f, 0f, distance);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(horizontalCoords, 0.5f);
    }
}
