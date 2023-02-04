using System;
using UnityEngine;

public class MagicRoot : MonoBehaviour
{
    [SerializeField] private CombatEntity endA;
    [SerializeField] private CombatEntity endB;

    public void OnEnable()
    {
        endA.onAttackReceived += ReceiveAttackOnA;
        endB.onAttackReceived += ReceiveAttackOnB;
    }
    
    public void OnDisable()
    {
        endA.onAttackReceived -= ReceiveAttackOnA;
        endB.onAttackReceived -= ReceiveAttackOnB;
    }

    private bool ReceiveAttackOnA(AttackProfileSO arg1, AttackInfo arg2)
    {
        throw new NotImplementedException();
    }

    private bool ReceiveAttackOnB(AttackProfileSO arg1, AttackInfo arg2)
    {
        throw new NotImplementedException();
    }
}