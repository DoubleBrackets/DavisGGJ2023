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
        float currentSpeed = heightBody.horizontalVel.magnitude;
        bool isOverTopSpeed =  currentSpeed > maxSpeed;
        bool isNoInput = input == Vector2.zero;
        
        // The guarantee 1 movespeed is to reduce collision issues with low speeds
        if (isNoInput || isOverTopSpeed)
        {
            if (currentSpeed <= 1f)
                heightBody.horizontalVel = Vector2.zero;
            ApplyHorizontalFriction(fricAccel, timeStep);
        }
        else
        {
            if (currentSpeed <= 1f)
                heightBody.horizontalVel = input * 1f;
            ApplyHorizontalMovement(input.normalized, maxSpeed, acceleration,fricAccel, timeStep);
        }
    }

    private void ApplyHorizontalMovement(Vector2 input, float maxSpeed, float acceleration, float frictionAccel, float timeStep)
    {
        Vector2 newVel = Vector2.MoveTowards(
            heightBody.horizontalVel, 
            input * maxSpeed, 
            acceleration * timeStep);

        if (newVel.magnitude < heightBody.horizontalVel.magnitude && frictionAccel > acceleration)
        {
            newVel = Vector2.MoveTowards(
                heightBody.horizontalVel, 
                input * maxSpeed, 
                frictionAccel * timeStep);
        }

        heightBody.horizontalVel = newVel;
    }

    private void ApplyHorizontalFriction(float fricAcceleration, float timeStep)
    {
        heightBody.horizontalVel = Vector2.MoveTowards(
            heightBody.horizontalVel,
            Vector2.zero,
            fricAcceleration * timeStep);
    }
}
