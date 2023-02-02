using System;
using UnityEngine;

public class HeightBody2D : MonoBehaviour
{
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

    private void OnEnable()
    {
        hasShadow = shadowTransform != null;
    }

    private void FixedUpdate()
    {
        ApplyForces();
        Vector2 horizontalStep = horizontalVel * Time.fixedDeltaTime;
        CalculateCollisions(ref horizontalStep);
        ApplyVelocity(horizontalStep);
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

    private void CalculateCollisions(ref Vector2 step)
    {
        VerticalCollisions();
        HorizontalCollisions(ref step, 0);
    }

    private void HorizontalCollisions(ref Vector2 step, int bounces)
    {
        if (bounces > maxResolves)
            return;
        Vector2 dir = step.normalized;
        float stepDist = step.magnitude;
        RecalculatePosition();
        
        var hit = Physics2D.BoxCast(
            physicsBox.bounds.center,
            physicsBox.size,
            0f,
            step,
            stepDist + skinWidth,
            wallMask,
            height,
            height + colliderHeight
            );

        if (hit)
        {
            float hitDist = hit.distance - skinWidth;
            
            Vector2 target = horizontalCoords + step;
            // Snap to the collision surface
            horizontalCoords += dir * hitDist;
            
            step = target - horizontalCoords;
            
            // "Slide" up the surface
            step = step.ProjectOntoNormal(hit.normal.normalized);
            
            horizontalVel = horizontalVel.ProjectOntoNormal(hit.normal);
            //horizontalVel += hit.normal * collisionOffset;
            step += hit.normal * collisionOffset;
            
            // Recursively resolve
            HorizontalCollisions(ref step, bounces + 1);
        }
    }

    private void ShadowCast()
    {
        
    }

    private void VerticalCollisions()
    {
        float heightStep = verticalVelocity * Time.fixedDeltaTime;
        isGrounded = false;
        // Check for ground
        var cast = Physics2D.BoxCast(
            physicsBox.bounds.center,
            physicsBox.size,
            0f,
            Vector2.down,
            Mathf.Abs(heightStep),
            groundMask,
            height + heightStep,
            height);

        // There is some ground here
        if (cast.collider != null)
        {
            var pos = cast.transform.position;
            float groundHeight = pos.z;
            height = groundHeight;
            verticalVelocity = 0f;
            isGrounded = true;
        }
    }

    private void ApplyVelocity(Vector2 horizontalStep)
    {
        horizontalCoords += horizontalStep;
        height += verticalVelocity * Time.fixedDeltaTime; 
    }

    private void RecalculatePosition()
    {
        transform.position = new Vector3(
            horizontalCoords.x,
            horizontalCoords.y + height,
            height);

        if (hasShadow)
        {
            shadowTransform.position = new Vector3(
                horizontalCoords.x,
                horizontalCoords.y + Mathf.Floor(height / 2f) * 2f,
                height);
        }
    }
}
