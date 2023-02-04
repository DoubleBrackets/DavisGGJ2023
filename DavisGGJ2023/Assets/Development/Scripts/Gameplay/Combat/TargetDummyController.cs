using System;
using UnityEngine;

public class TargetDummyController : MonoBehaviour
{
    [SerializeField] private CombatEntity combatEntity;
    [SerializeField] private HeightBody2D heightBody;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite staggeredSprite;

    [SerializeField] private PlayVFXFuncChannelSO askPlayVFX;
    [SerializeField] private VFXEffectProfile deathVFX;

    private enum TargetDummyState
    {
        Normal,
        Staggered
    }

    private float staggerDuration = 0f;
    private TargetDummyState currentState;
    
    private void OnEnable()
    {
        combatEntity.onAttackReceived += ReceiveAttack;
        heightBody.onHitHazard += OnHazard;
        ExitStagger();
    }
    
    private void OnDisable()
    {
        combatEntity.onAttackReceived -= ReceiveAttack;
        heightBody.onHitHazard -= OnHazard;
    }

    private bool ReceiveAttack(AttackProfileSO attackProfile, AttackInfo attack)
    {
        if (currentState == TargetDummyState.Staggered) return false;
        
        heightBody.horizontalVel = 
            attack.attackAngle * Vector2.right
            * attackProfile.KnockbackVelocity;

        heightBody.verticalVelocity = attackProfile.VerticalKnockbackVelocity;

        // Do not invoke if stagger is being extended
        if (staggerDuration <= 0f && attackProfile.KnockbackStaggerDuration > 0f)
        {
            EnterStagger();
        }
        
        staggerDuration = Mathf.Max(staggerDuration, attackProfile.KnockbackStaggerDuration);

        if (attackProfile.Damage > 0)
        {
            // Do nothing
        }
        
        return true;
    }

    private void FixedUpdate()
    {
        if (staggerDuration > 0f)
        {
            staggerDuration -= Time.fixedDeltaTime;
            if (staggerDuration <= 0f)
            {
                ExitStagger();
            }
        }
        else
        {
            heightBody.horizontalVel = Vector2.MoveTowards(
                heightBody.horizontalVel, 
                Vector2.zero, 
                40f * Time.fixedDeltaTime);
        }
    }

    private void EnterStagger()
    {
        currentState = TargetDummyState.Staggered;
        spriteRenderer.sprite = staggeredSprite;
        heightBody.onHorizontalCollide += OnKilled;
    }
    
    private void ExitStagger()
    {
        currentState = TargetDummyState.Normal;
        spriteRenderer.sprite = normalSprite;
        heightBody.horizontalVel *= 0.5f;
        heightBody.onHorizontalCollide -= OnKilled;
    }

    private void OnHazard()
    {
        OnKilled(Vector2.zero, Vector2.up);
    }

    private void OnKilled(Vector2 collisionVel, Vector2 initialVel)
    {
        Vector3 pos = heightBody.TransformPosition;
        pos += initialVel.GetAngle() * Vector3.right;
        askPlayVFX.CallFunc(deathVFX, 0, new PlayVFXSettings()
        {
            position = pos,
            rotation = initialVel.GetAngle()
        });
        Destroy(gameObject);
    }
}
