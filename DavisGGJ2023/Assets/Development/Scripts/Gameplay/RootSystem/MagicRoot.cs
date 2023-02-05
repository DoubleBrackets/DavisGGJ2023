using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class MagicRoot : MonoBehaviour
{
    [ColorHeader("Sending Attacks To")]
    [SerializeField] private MagicRoot[] sendingAttacksTo;

    [ColorHeader("Attack Config - Sending", ColorHeaderColor.Config)]
    [SerializeField] private float attackSendDelay;
    
    [ColorHeader("Attack Config - Receiving", ColorHeaderColor.Config)]
    [SerializeField] private AttackProfileSO rootAttackProfile;
    [SerializeField] private float attackReceiveDelay;
    [SerializeField] private Transform attackSource;
    [SerializeField] private VFXEffectProfile attackVFX;
    [SerializeField] private float knockbackRatio;

    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels)]
    [SerializeField] private PlayVFXFuncChannelSO askPlayVFX;
    [SerializeField] private PerformAttackFuncChannelSO askPerformAttack;

    [ColorHeader("Warping To", showDivider: true)]
    [SerializeField] private MagicRoot warpingTo;
    
    [ColorHeader("Warping Config", ColorHeaderColor.Config)]
    [SerializeField] private VFXEffectProfile warpVFX;
    [SerializeField] private float travelTime;

    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels)]
    [SerializeField] private RootWarpEventChannelSO askWarpPlayer;

    [ColorHeader("Dependencies", showDivider: true)]
    [SerializeField] private CombatEntity rootHitboxTarget;

    public void OnEnable()
    {
        rootHitboxTarget.onAttackReceived += ReceiveInteraction;
    }
    
    public void OnDisable()
    {
        rootHitboxTarget.onAttackReceived -= ReceiveInteraction;
    }

    private bool ReceiveInteraction(AttackProfileSO attackProfile, AttackInfo attackInfo)
    {
        // Check whether this attack is a basic attack or a teleport attempt
        if (attackProfile.Tag == 0)
        {
            bool attackSent = false;
            // Its an attack - propogate the attack 
            foreach (var root in sendingAttacksTo)
            {
                if (root)
                {
                    attackSent = true;
                    root.PerformRootAttack(attackInfo, attackSendDelay);
                }
            }
            if(attackSent)
                return true;
        }
        else if (attackProfile.Tag == 1 && warpingTo != null)
        {
            // Its a warp - send the warp request
            askPlayVFX.CallFunc(
                warpVFX,
                0,
                new PlayVFXSettings
                {
                    position = transform.position,
                    rotation = transform.rotation
                }
            );
            warpingTo.GetWarpedTo(travelTime);
            return true;
        }

        return false;
    }

    private void GetWarpedTo(float travelTime)
    {
        StartCoroutine(CoroutWarpPlayerToHere(travelTime));
        
    }

    private IEnumerator CoroutWarpPlayerToHere(float travelTime)
    {
        var rawPosition = transform.position;
        var warpPosition = rawPosition - Vector3.up * rawPosition.z;
        askWarpPlayer.RaiseEvent(warpPosition, travelTime);
        yield return new WaitForSeconds(travelTime);
        askPlayVFX.CallFunc(
            warpVFX,
            0,
            new PlayVFXSettings
            {
                position = rawPosition,
                rotation = transform.rotation
            }
        );
    }

    private void PerformRootAttack(AttackInfo attackInfo, float delay)
    {
        StartCoroutine(CoroutRootAttack(rootAttackProfile, attackInfo, delay));
    }

    private IEnumerator CoroutRootAttack(AttackProfileSO attackProfile, AttackInfo attackInfo, float delay)
    {
        yield return new WaitForSeconds(attackProfile.PlayVFXTime + delay + attackReceiveDelay);
                
        var rotation = attackSource.rotation;
        var position = attackSource.position;
        
        askPlayVFX.CallFunc(
            attackVFX,
            0,
            new PlayVFXSettings
            {
                position = position,
                rotation = rotation
            }
        );

        yield return new WaitForSeconds(attackProfile.WindupDuration - attackProfile.PlayVFXTime);

        askPerformAttack.CallFunc(
            attackProfile,
            new AttackInfo
            {
                ignoreSource = rootHitboxTarget.gameObject,
                attackSourcePositionRaw = position,
                attackAngle = rotation,
                knockbackRatio = knockbackRatio
            }
        );

    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 pos = transform.position;
        if (warpingTo)
        {
            Gizmos.DrawLine(pos + Vector3.down * 0.1f, warpingTo.transform.position + Vector3.down * 0.1f);
            Gizmos.DrawWireSphere(Vector3.Lerp(pos, warpingTo.transform.position, 0.75f), 0.25f);
        }

        if (sendingAttacksTo != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var attackTarget in sendingAttacksTo)
            {
                if (attackTarget)
                {
                    Gizmos.DrawLine(pos, attackTarget.transform.position);
                    Gizmos.DrawWireSphere(Vector3.Lerp(pos, attackTarget.transform.position, 0.75f), 0.15f);
                }
            }
        }

        Gizmos.color = Color.red;
        if (attackSource)
        {
            var position = attackSource.position;
            Gizmos.DrawWireCube(position, Vector3.one / 5f);
            Gizmos.DrawLine(position, position + attackSource.rotation * Vector3.right);
        }
        
        
        Handles.Label(
            pos + 0.5f * Vector3.up - Vector3.right, 
            $"RootPos: {pos - Vector3.up * pos.z}");
    }
    #endif
}