using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class SprintMovementState : MovementStateBase
    {
        LocomotionController controller;
        public SprintMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            controller = ctx.GetComponent<LocomotionController>();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (!Data.IsAnimationDrivenMovement)
            {
                if (_character.Input.MovementInput.magnitude > 0)
                    controller.Move();

            }
            if (!Data.IsAnimationDrivenRotation)
            {
                _character.Rotate();

            }
        }

        public override bool CanExit()
        {
            return !_character.Input.SprintPressing || _character.Input.MovementInput.magnitude == 0;
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Sensors.IsGrounded &&
                        _character.Input.SprintPressing;
        }
    }

}
