using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;

namespace LOGIYGames.Movement
{
    public class GroundJumpMovementState : TimedMovementState
    {
        JumpController controller;
        private JumpStateData _stateData;
        public GroundJumpMovementState(Actor ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;
            controller = ctx.GetComponent<JumpController>();
        }
        public override void Enter()
        {
            base.Enter();
            Direction direction = _character.GetRelativeMovementDirection();
            float planarForce = _stateData.PlanarJumpForce;
            if (_character.Input.MovementInput.magnitude == 0)
            {
                direction = Direction.Up;
                planarForce = 0;
            }
            _character.EventBus.Publish(new JumpPerformedEvent
            {
                verticalForce = _stateData.VerticalJumpForce,
                planarForce = planarForce,
                direction = direction,
                jumpType = JumpType.GroundJump
            });
        }
        public override bool CanEnter()
        {
            return base.CanEnter()
                && (_character.Sensors.IsValidSlope() || _character.Sensors.GroundAngle <= 0)
                && _character.Input.JumpPressed
                && controller.CanExecute(_stateData);
        }

    }
}
