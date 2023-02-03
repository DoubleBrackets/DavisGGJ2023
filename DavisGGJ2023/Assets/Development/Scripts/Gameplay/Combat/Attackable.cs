using UnityEngine;

public interface Attackable
{
    public bool ReceiveAttack(AttackProfileSO attackProfile, AttackInfo attack);
}
