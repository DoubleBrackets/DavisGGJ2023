using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class MagicRoot : MonoBehaviour
{
    [ColorHeader("Sending Attacks To")]
    [SerializeField] private MagicRoot[] sendingAttacksTo;

    [ColorHeader("Attack Config", ColorHeaderColor.Config)]
    [SerializeField] private VFXEffectProfile attackVFX;
    [SerializeField] private Transform attackSource;
    [SerializeField] private float attackDelay;

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
            // Its an attack - propogate the attack 
            foreach (var root in sendingAttacksTo)
            {
                if (root)
                {
                    root.PerformRootAttack(attackProfile, attackInfo, attackDelay);
                }
            }
            if(sendingAttacksTo.Length > 0)
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

    private void PerformRootAttack(AttackProfileSO attackProfile, AttackInfo attackInfo, float delay)
    {
        StartCoroutine(CoroutRootAttack(attackProfile, attackInfo, delay));
    }

    private IEnumerator CoroutRootAttack(AttackProfileSO attackProfile, AttackInfo attackInfo, float delay)
    {
        yield return new WaitForSeconds(attackProfile.PlayVFXTime + delay);
                
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
                attackSourcePosition = position,
                attackAngle = rotation,
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
            Gizmos.DrawLine(pos, warpingTo.transform.position);
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