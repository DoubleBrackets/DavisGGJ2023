using UnityEngine;

public class ProtagAnimator : MonoBehaviour
{
    [ColorHeader("Dependencies")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRen;

    private string currentPlaying;

    public enum Facing
    {
        Down,
        Side,
        Up
    }

    private Facing currentFacing = Facing.Down;

    public void SetFacing(Facing facing)
    {
        currentFacing = facing;
    }

    public void SetFacing(Vector2 dir)
    {
        if (dir == Vector2.zero) return;

        if (Mathf.Abs(dir.x) > 0f)
        {
            currentFacing = Facing.Side;
            spriteRen.flipX = dir.x < 0f;
        }
        else if (dir.y > 0)
        {
            currentFacing = Facing.Up;
            spriteRen.flipX = false;
        }
        else
        {
            currentFacing = Facing.Down;
            spriteRen.flipX = false;
        }
    }

    public void PlayAnimation(string animName)
    {
        string state = $"Protag{currentFacing.ToString()}_{currentFacing.ToString()}{animName}";
        if (state == currentPlaying) return;
        currentPlaying = state;
        animator.Play(state, 0);
    }
}
