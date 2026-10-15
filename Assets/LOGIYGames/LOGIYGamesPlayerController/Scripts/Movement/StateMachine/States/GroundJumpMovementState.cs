using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames.Movement
{
    public class GroundJumpMovementState : TimedMovementState
    {
        JumpController controller;
        private JumpStateData _jumpStateData;
        public GroundJumpMovementState(Actor ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _jumpStateData = stateData;
            controller = ctx.GetComponent<JumpController>();
        }
        public override void Enter()
        {
            base.Enter();

            float verticalForce;
            float planarForce;
            JumpType jumpType = JumpType.GroundJump;
            if (!Data.IsAnimationDrivenMovement)
            {
                verticalForce = _jumpStateData.VerticalJumpForce;
                planarForce = _jumpStateData.PlanarJumpForce;
            }
            else
            {
                verticalForce = 0;
                planarForce = 0;
            }
            _character.EventBus.Publish(new JumpPerformedEvent
            {
                jumpType = jumpType,
                planarForce = planarForce,
                verticalForce = verticalForce,
                direction = _character.GetInputRelativeDirection()
            });
            controller.Jump(jumpType,verticalForce,planarForce);

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
                && controller.CanExecute(_jumpStateData);
        }

    }
}
