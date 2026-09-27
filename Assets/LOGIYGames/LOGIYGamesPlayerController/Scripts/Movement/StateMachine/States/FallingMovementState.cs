using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class FallingMovementState : MovementStateBase
    {
        public FallingMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }

        public override void Enter()
        {

            base.Enter();
            _character.MovementStrategy = new StrafeMovement(_character);
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (_controller.Velocity.magnitude < _character.RuntimeMovement.CurrentSpeed * 3.5f)
            {

                _controller.AddAcceleration(_character.RuntimeMovement.TargetDirection * _character.RuntimeMovement.CurrentSpeed * 3.5f);
            }

            _character.Rotate();
        }
        public override void Exit()
        {
            base.Exit();
        }
    }

}
