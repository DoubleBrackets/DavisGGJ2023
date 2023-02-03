using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Profiles/AttackProfileSO")]
public class AttackProfileSO : ScriptableObject
{
    public float KnockbackVelocity => knockbackVelocity;

    public float VerticalKnockbackVelocity => verticalKnockbackVelocity;

    public float Damage => damage;

    public float WindupDuration => windupDuration;

    public float FollowThroughDuration => followThroughDuration;

    public float FreezeFrameDuration => freezeFrameDuration;

    public float KnockbackStaggerDuration => knockbackStaggerDuration;

    [ColorHeader("Stats")]
    [SerializeField] private float knockbackVelocity;
    [SerializeField] private float verticalKnockbackVelocity;
    [SerializeField] private float knockbackStaggerDuration;
    [SerializeField] private float damage;
    
    [ColorHeader("Timing")]
    [SerializeField] private float windupDuration;
    [SerializeField] private float followThroughDuration;

    [ColorHeader("Effects")]
    [SerializeField] private float freezeFrameDuration;
}
