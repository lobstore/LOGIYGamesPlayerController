using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class RunMovementState : MovementStateBase
    {
        LocomotionController controller;
        public RunMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData) {
            controller = ctx.GetComponent<LocomotionController>();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (!Data.IsAnimationDrivenMovement)
            {
                controller.Move();

            }
            if (!Data.IsAnimationDrivenRotation)
            {
                _character.Rotate();

            }
        }

        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Input.MovementInput.magnitude > 0 && _character.IsGrounded;
        }
    }

}
