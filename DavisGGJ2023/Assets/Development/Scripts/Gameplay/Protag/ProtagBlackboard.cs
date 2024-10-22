using UnityEngine;

public class ProtagBlackboard : DescriptionMonoBehavior
{
    [ColorHeader("Dependencies")]
    [SerializeField] public ProtagInputProvider InputProvider;
    [SerializeField] public PlayerInputState InputState;
    [SerializeField] public GameStateSO gameState;
    
    [ColorHeader("Visuals")]
    [SerializeField] public ProtagAnimator animator;
    
    [ColorHeader("Movement/Physics")]
    [SerializeField] public BasicMovement ProtagMover;
    [SerializeField] public HeightBody2D heightBody;
    [SerializeField] public BasicMovementProfileSO basicMovementProfile;
    [SerializeField] public Transform playerBodyTransform;

    [ColorHeader("Combat")]
    [SerializeField] public AttackProfileSO basicAttackProfile;
    [SerializeField] public AttackProfileSO tryRootWarpAttackProfile;
    [SerializeField] public CombatEntity protagCombatEntity;

    [ColorHeader("VFX")]
    [SerializeField] public VFXEffectProfile basicAttackVFX;
    [SerializeField] public VFXEffectProfile deathVFX;

    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels, true)]
    [ColorHeader("Combat")]
    [SerializeField] public PerformAttackFuncChannelSO askPerformAttack;
    
    [ColorHeader("Camera")]
    [SerializeField] public VoidEventChannelSO askStopFollowingTarget;
    [SerializeField] public TransformEventChannelSO askStartFollowingTarget;
    [SerializeField] public FloatEventChannelSO askSetDamping;

    [ColorHeader("VFX")]
    [SerializeField] public PlayVFXFuncChannelSO askPlayVFX;
    [SerializeField] public FloatEventChannelSO askFreezeFrame;

    [ColorHeader("Level Control")]
    [SerializeField] public VoidEventChannelSO askRestartLevel;

    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels, true)]
    [ColorHeader("Combat")]
    [SerializeField] public RootWarpEventChannelSO askPerformRootWarp;

    [ColorHeader("State", showDivider: true)]
    [ReadOnly] public float basicAttackFinishTime;
    [ReadOnly] public float warpTravelTime;
    [ReadOnly] public Vector3 warpTarget;

    [ReadOnly] public AttackProfileSO recentAttacked;
    [ReadOnly] public AttackInfo recentAttackedInfo;

    public void UpdateInput()
    {
        InputProvider.GetInputState(ref InputState);
    }
}
