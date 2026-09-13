using LOGIYGames.CharacterCore;
using UnityEngine;

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
            if (_character.Input.MovementInput.magnitude > 0)
            {

                _controller.ChangeVelocity(_controller.Velocity+_character.RuntimeMovement.TargetDirection * _character.RuntimeMovement.CurrentSpeed * 3.5f * Time.deltaTime);

            }
        }
        public override void Exit()
        {
            base.Exit();
        }
    }

}
