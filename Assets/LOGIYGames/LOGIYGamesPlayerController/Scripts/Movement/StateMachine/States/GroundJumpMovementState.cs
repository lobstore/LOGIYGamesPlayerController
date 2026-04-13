using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames.Movement
{
    public class GroundJumpMovementState : TimedMovementState
    {
        private JumpStateData _stateData;
        public GroundJumpMovementState(Character ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;
        }
        public override void Enter()
        {
            base.Enter();
            Direction direction = _character.GetRelativeMovementDirection();
            if (_character.Input.MovementInput.magnitude ==0)
            {
                direction = Direction.Up;
            }
            _character.EventBus.Publish(new JumpPerformedEvent
            {
                verticalForce = _stateData.VerticalJumpForce,
                planarForce = _stateData.PlanarJumpForce,
                direction = direction,
                jumpType = JumpType.GroundJump

            });
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && (_character.Sensors.IsValidSlope() || _character.Sensors.GroundAngle<=0);
        }

    }
}
