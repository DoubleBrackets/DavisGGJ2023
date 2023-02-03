using UnityEngine;

public class ProtagBlackboard : DescriptionMonoBehavior
{
    [ColorHeader("Dependencies")]
    [SerializeField] public ProtagInputProvider InputProvider;
    [SerializeField] public PlayerInputState InputState;
    [SerializeField] public BasicMovement ProtagMover;
    [SerializeField] public HeightBody2D heightBody;
    [SerializeField] public BasicMovementProfileSO basicMovementProfile;

    [ColorHeader("State")]
    [ReadOnly] public float time;
    

    public void UpdateInput()
    {
        InputProvider.GetInputState(ref InputState);
    }
}
