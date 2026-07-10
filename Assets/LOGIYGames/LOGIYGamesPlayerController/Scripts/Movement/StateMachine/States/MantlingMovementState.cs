using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine.Events;

namespace LOGIYGames
{
    public class MantlingMovementState : CharacterMovementState
    {
        #region Fields
        private MantlingController MantlingController;

        public UnityEvent OnMantlingStart = new UnityEvent();
        public UnityEvent OnMantlingEnd = new UnityEvent();
        #endregion

        public MantlingMovementState(Character ctx, MantlingMovmentStateData stateData) : base(ctx, stateData)
        {
            MantlingController = ctx.MantlingController;
        }


        public override void Enter()
        {
            base.Enter();
            _character.RotationStrategy = new NoneRotation(_character);
            _character.MovementStrategy = new NoneMovement();
            _controller.UseGravity = false;
            _controller.IsNoClip = true;
            MantlingController.BeginMantling();
            OnMantlingStart.Invoke();
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            MantlingController.Tick();
        }
        public override void Exit()
        {
            base.Exit();
            MantlingController.Cancel();

            _controller.UseGravity = true;
            _controller.IsNoClip = false;
            OnMantlingEnd.Invoke();
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && MantlingController.CanEnter();
        }
        public bool CanExit()
        {
            return MantlingController.CanExit();
        }
    }
}