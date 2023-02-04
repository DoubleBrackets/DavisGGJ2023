using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private PerformAttackFuncChannelSO askPerformAttack;

    [ColorHeader("Debug")]
    [SerializeField] private AttackProfileSO hitboxViewer;
    private List<DebugHitbox> toDebug = new ();

    private class DebugHitbox
    {
        public Vector2 center;
        public Vector2 size;
        public Quaternion rotation;
        public float createTime;
    }
    
    private void OnEnable()
    {
        askPerformAttack.OnCalled += PerformAttack;
    }
    
    private void OnDisable()
    {
        askPerformAttack.OnCalled -= PerformAttack;
    }

    private bool PerformAttack(AttackProfileSO attackProfile, AttackInfo attackInfo)
    {
        Vector3 castPos = attackInfo.attackSourcePosition;
        
        if (attackProfile.AttackFromEdge)
        {
            float extentsX = attackProfile.Width / 2f;
            castPos += (Vector3)attackInfo.attackAngle.GetVector() * extentsX;
        }

        Vector2 size = new Vector2(attackProfile.Width, attackProfile.Height);
        float angle = attackInfo.attackAngle.eulerAngles.z;
        
        // Cast
        var hits = Physics2D.OverlapBoxAll(
            castPos,
            size,
            angle,
            attackProfile.HitMask,
            castPos.z - attackProfile.Depth,
            castPos.z + attackProfile.Depth
        );

        bool anyHits = false;

        foreach (var hit in hits)
        {
            var combatEntity = hit.GetComponent<Attackable>();
            if (combatEntity != null)
            {
                anyHits |= combatEntity.ReceiveAttack(attackProfile, attackInfo);
            }
        }

        #if UNITY_EDITOR
        toDebug.Add(new DebugHitbox()
        {
            center = castPos,
            size = size,
            rotation = attackInfo.attackAngle,
            createTime = Time.time
        });
        #endif
        
        return anyHits;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (hitboxViewer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, 
                new Vector2(hitboxViewer.Width, hitboxViewer.Height));
        }

        for(int i = 0;i < toDebug.Count;i++)
        {
            var box = toDebug[i];
            Gizmos.color = Color.red;
            Vector2 extents = box.size / 2f;
            Vector2 extentsDiag = extents;
            extentsDiag.x *= -1;

            extents = box.rotation * extents;
            extentsDiag = box.rotation * extentsDiag;
            
            
            Gizmos.DrawLine(
                box.center + extents, 
                box.center - extents);
            
            Gizmos.DrawLine(
                box.center + extentsDiag, 
                box.center - extentsDiag);

            if (Time.time - box.createTime > 0.5f)
            {
                toDebug.RemoveAt(i);
                i--;
            }
        }
    }
    #endif
}
