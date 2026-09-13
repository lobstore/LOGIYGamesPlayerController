using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class CrouchMovementState : MovementStateBase
    {
        protected float StandingHeight;
        protected float CrouchHeight;

        public CrouchMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            StandingHeight = _controller.Height;
            CrouchHeight = StandingHeight * 0.5f;
        }

        public override void Enter()
        {
            base.Enter();
            _controller.Height = CrouchHeight;
        }

        public override void Exit()
        {
            base.Exit();
            _controller.Height = StandingHeight;
        }
    }

}
