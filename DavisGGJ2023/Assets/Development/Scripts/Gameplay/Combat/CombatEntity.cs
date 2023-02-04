using System;
using UnityEngine;

public class CombatEntity : MonoBehaviour, Attackable
{
    // Events
    public event Func<AttackProfileSO, AttackInfo, bool> onAttackReceived;

    public bool ReceiveAttack(AttackProfileSO attackProfile, AttackInfo attack)
    {
        if (onAttackReceived == null)
        {
            return false;
        }
        return onAttackReceived.Invoke(attackProfile, attack);
    }
}

public struct AttackInfo
{
    public Vector3 attackSourcePosition;
    public Quaternion attackAngle;
    public int tag;
}
