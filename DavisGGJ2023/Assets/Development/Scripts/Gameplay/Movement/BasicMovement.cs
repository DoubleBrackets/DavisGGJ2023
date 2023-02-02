using UnityEngine;

/// <summary>
/// Simple movement with simulated height
/// </summary>
public class BasicMovement : MonoBehaviour
{
    [ColorHeader("Dependencies")]
    [SerializeField] private HeightBody2D heightBody;

    public void SimpleHorizontalMovement(
        Vector2 input,
        float maxSpeed,
        float acceleration,
        float fricAccel,
        float timeStep)
    {
        bool isOverTopSpeed = heightBody.horizontalVel.magnitude > maxSpeed;
        bool isNoInput = input == Vector2.zero;
        
        if (isNoInput || isOverTopSpeed)
        {
            ApplyHorizontalFriction(fricAccel, timeStep);
        }
        else
        {
            ApplyHorizontalMovement(input.normalized, maxSpeed, acceleration, timeStep);
        }
    }

    private void ApplyHorizontalMovement(Vector2 input, float maxSpeed, float acceleration, float timeStep)
    {
        heightBody.horizontalVel = Vector2.MoveTowards(
            heightBody.horizontalVel, 
            input * maxSpeed, 
            acceleration * timeStep);
    }

    private void ApplyHorizontalFriction(float fricAcceleration, float timeStep)
    {
        heightBody.horizontalVel = Vector2.MoveTowards(
            heightBody.horizontalVel,
            Vector2.zero,
            fricAcceleration * timeStep);
    }
}
