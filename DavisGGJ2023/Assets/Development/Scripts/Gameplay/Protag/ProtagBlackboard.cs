using UnityEngine;

public class ProtagBlackboard : DescriptionMonoBehavior
{
    [ColorHeader("Dependencies")]
    [SerializeField] public ProtagInputProvider InputProvider;
    [SerializeField] public PlayerInputState InputState;
    
    [ColorHeader("Visuals")]
    [SerializeField] public ProtagAnimator animator;
    
    [ColorHeader("Movement/Physics")]
    [SerializeField] public BasicMovement ProtagMover;
    [SerializeField] public HeightBody2D heightBody;
    [SerializeField] public BasicMovementProfileSO basicMovementProfile;
    [SerializeField] public Transform playerBodyTransform;

    [ColorHeader("Gameplay")]
    [SerializeField] public AttackProfileSO basicAttackProfile;

    [ColorHeader("VFX")]
    [SerializeField] public PlayVFXProfile basicAttackVFX;

    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels)]
    [ColorHeader("Camera")]
    [SerializeField] public VoidEventChannelSO askStopFollowingTarget;
    [SerializeField] public TransformEventChannelSO askStartFollowingTarget;

    [ColorHeader("VFX")]
    [SerializeField] public PlayVFXFuncChannelSO askPlayVFX;
    [SerializeField] public FloatEventChannelSO askFreezeFrame;
    
    [ColorHeader("State")]
    [ReadOnly] public float time;
    

    public void UpdateInput()
    {
        InputProvider.GetInputState(ref InputState);
    }
}
