using System;
using UnityEngine;

public class CombatEntity : MonoBehaviour, Attackable
{
    [ColorHeader("Dependencies")]
    [SerializeField] private HeightBody2D heightBody;
    
    // Events
    public event Action onStaggerStarted;
    public event Action onStaggerEnded;
    
    // Fields
    private bool canBeHit;
    private float staggerDuration;

    public bool ReceiveAttack(AttackProfileSO attackProfile, AttackInfo attack)
    {
        if (!canBeHit) return false;
        
        heightBody.horizontalVel = 
            (heightBody.TransformPosition - attack.attackSourcePosition) 
            * attackProfile.KnockbackVelocity;

        heightBody.verticalVelocity = attackProfile.VerticalKnockbackVelocity;

        // Do not invoke if stagger is being extended
        if (staggerDuration <= 0f)
        {
            onStaggerStarted?.Invoke();
        }
        staggerDuration = Mathf.Max(staggerDuration, attackProfile.KnockbackStaggerDuration);
        
        return true;
    }

    public void SetInvulnerable(bool val)
    {
        canBeHit = val;
    }

    private void Update()
    {
        if (staggerDuration > 0f)
        {
            staggerDuration -= Time.deltaTime;
            if (staggerDuration <= 0f)
            {
                onStaggerEnded?.Invoke();
            }
        }
    }
}

public struct AttackInfo
{
    public Vector2 attackSourcePosition;
}
