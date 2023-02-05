using System;
using UnityEngine;

public class BasicEnemyProjectile : MonoBehaviour
{
    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels)]
    [SerializeField] private PerformAttackFuncChannelSO askPeformAttack;
    [SerializeField] private PlayVFXFuncChannelSO askPlayVFX;
    
    [ColorHeader("Dependencies", showDivider:true)]
    [SerializeField] private HeightBody2D heightBody;
    [SerializeField] private AttackProfileSO explodeAttackProfile;
    [SerializeField] private VFXEffectProfile explodeVFX;
    


    private void OnEnable()
    {
        heightBody.onHitGround += Explode;
        heightBody.onHitHazard += Explode;
        heightBody.onHorizontalCollide += Explode;
        heightBody.horizontalCollideEnabled = false;
    }
    
    private void OnDisable()
    {
        heightBody.onHitGround -= Explode;
        heightBody.onHitHazard -= Explode;
        heightBody.onHorizontalCollide -= Explode;
    }

    private void Explode()
    {
        askPeformAttack.CallFunc(
            explodeAttackProfile,
            new AttackInfo
            {
                attackSourcePositionRaw = heightBody.TransformPosition
            }
        );

        askPlayVFX.CallFunc(
            explodeVFX,
            0,
            new PlayVFXSettings
            {
                position = heightBody.TransformPosition
            }
        );
        
        Destroy(gameObject);
    }

    private void Explode(Vector2 arg1, Vector2 arg2)
    {
        Explode();
    }

    public void SetVelocity(Vector2 horizontal, float vertical, float gravityAccel)
    {
        heightBody.horizontalVel = horizontal;
        heightBody.verticalVelocity = vertical;
        heightBody.terminalVelocity = 100000f;
        heightBody.gravityAccel = gravityAccel;
    }

    public void SetPosition(Vector2 horizontal, float vertical)
    {
        heightBody.horizontalPos = horizontal;
        heightBody.height = vertical;
        heightBody.ForceUpdate();
    }
}
