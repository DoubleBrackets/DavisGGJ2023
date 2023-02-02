using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [ColorHeader("Dependencies")]
    [SerializeField] private Rigidbody2D rb;

    public void PerformSimpleMovement(
        Vector2 input,
        float maxSpeed,
        float acceleration,
        float fricAccel,
        float timeStep)
    {
        bool isOverTopSpeed = rb.velocity.magnitude > maxSpeed;
        bool isNoInput = input == Vector2.zero;
        
        if (isNoInput || isOverTopSpeed)
        {
            ApplyFriction(fricAccel, timeStep);
        }
        else
        {
            ApplyMovement(input.normalized, maxSpeed, acceleration, timeStep);
        }
    }

    private void ApplyMovement(Vector2 input, float maxSpeed, float acceleration, float timeStep)
    {
        rb.velocity = Vector2.MoveTowards(
            rb.velocity, 
            input * maxSpeed, 
            acceleration * timeStep);
    }

    private void ApplyFriction(float fricAcceleration, float timeStep)
    {
        rb.velocity = Vector2.MoveTowards(
            rb.velocity,
            Vector2.zero,
            fricAcceleration * timeStep);
    }
}
