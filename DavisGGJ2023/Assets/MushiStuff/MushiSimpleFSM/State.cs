namespace MushiSimpleFSM
{
    public interface State
    {
        public void UpdateState();
        public void FixedUpdateState();
        public void EnterState();
        public void ExitState();
    }
}