using System;
using UnityEngine;

public class ContactKiller : MonoBehaviour
{
    [ColorHeader("Config", ColorHeaderColor.Config)]
    [SerializeField] private Collider2D thisColl;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private AttackProfileSO attackProfile;
    
    private void OnTriggerStay2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) | hitMask) == hitMask)
        {

            if (Mathf.Abs(col.bounds.center.z - thisColl.bounds.center.z) <= attackProfile.Depth)
            {
                var combatBody = col.GetComponent<CombatEntity>();
                if (combatBody)
                {
                    combatBody.ReceiveAttack(
                        attackProfile,
                        new AttackInfo
                        {
                            attackSourcePositionRaw = thisColl.bounds.center
                        }
                    );
                }
            }
        }
    }
}
