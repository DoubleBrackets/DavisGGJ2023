#region

using System;

#endregion

namespace MushiSimpleFSM
{
    public class QState : State
    {
        private Action updateAction;
        private Action fixedUpdateAction;
        private Action enterStateAction;
        private Action exitStateAction;

        private Func<int> switchStateAction;

        // Should update the same frame it is entered
        private bool immediateUpdate;
        public bool ImmediateUpdate => immediateUpdate;

        public QState(
            Action updateAction,
            Action fixedUpdateAction,
            Action enterStateAction,
            Action exitStateAction,
            Func<int> switchStateAction,
            bool immediateUpdate)
        {
            this.exitStateAction = exitStateAction;
            this.updateAction = updateAction;
            this.fixedUpdateAction = fixedUpdateAction;
            this.enterStateAction = enterStateAction;
            this.switchStateAction = switchStateAction;
            this.immediateUpdate = immediateUpdate;
        }

        public void UpdateState()
        {
            updateAction?.Invoke();
        }

        public void FixedUpdateState()
        {
            fixedUpdateAction?.Invoke();
        }

        public void EnterState()
        {
            enterStateAction?.Invoke();
        }

        public void ExitState()
        {
            exitStateAction?.Invoke();
        }

        public int SwitchState()
        {
            return switchStateAction?.Invoke() ?? -1;
        }
    }
}