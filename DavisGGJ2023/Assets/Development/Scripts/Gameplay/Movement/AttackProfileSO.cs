using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Profiles/AttackProfileSO")]
public class AttackProfileSO : ScriptableObject
{
    public float Width => width;

    public float Height => height;

    public float Cooldown => cooldown;

    public float KnockbackVelocity => knockbackVelocity;

    public float VerticalKnockbackVelocity => verticalKnockbackVelocity;

    public float Damage => damage;

    public float WindupDuration => windupDuration;

    public float FollowThroughDuration => followThroughDuration;

    public float FreezeFrameDuration => freezeFrameDuration;

    public float KnockbackStaggerDuration => knockbackStaggerDuration;

    public bool AttackFromEdge => attackFromEdge;

    public LayerMask HitMask => hitMask;

    public float Depth => depth;

    public float PlayVFXTime => playVFXTime;

    [ColorHeader("Stats")]
    [SerializeField] private float knockbackVelocity;
    [SerializeField] private float verticalKnockbackVelocity;
    [SerializeField] private float knockbackStaggerDuration;
    [SerializeField] private float damage;

    [ColorHeader("Hitbox")]
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float depth;
    [SerializeField] private bool attackFromEdge;
    [SerializeField] private LayerMask hitMask;
    
    [ColorHeader("Timing")]
    [SerializeField] private float playVFXTime;
    [SerializeField] private float windupDuration;
    [SerializeField] private float followThroughDuration;
    [SerializeField] private float cooldown;

    [ColorHeader("Effects")]
    [SerializeField] private float freezeFrameDuration;
}
