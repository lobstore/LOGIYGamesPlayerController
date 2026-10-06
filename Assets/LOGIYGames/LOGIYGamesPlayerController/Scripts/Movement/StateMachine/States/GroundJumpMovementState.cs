using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;

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

            float planarForce = _stateData.PlanarJumpForce;
            if (!Data.IsAnimationDrivenMovement)
            {
                _character.EventBus.Publish(new JumpPerformedEvent
                {
                    jumpType = JumpType.GroundJump,
                    planarForce = _stateData.PlanarJumpForce,
                    verticalForce = _stateData.VerticalJumpForce,
                    direction = _character.GetInputRelativeDirection()
                });
            }
            else
            {
                _character.EventBus.Publish(new JumpPerformedEvent
                {
                    jumpType = JumpType.GroundJump,
                    planarForce = 0,
                    verticalForce = 0,
                    direction = _character.GetInputRelativeDirection()
                });
            }

        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _character.Rotate();
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
