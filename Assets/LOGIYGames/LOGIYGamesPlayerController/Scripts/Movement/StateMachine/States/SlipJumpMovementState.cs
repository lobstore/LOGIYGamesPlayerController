using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames
{
    public class SlipJumpMovementState : TimedMovementState
    {
        private JumpStateData _jumpStateData;
        JumpController controller;
        public SlipJumpMovementState(Actor ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _jumpStateData = stateData;
            controller = ctx.GetComponent<JumpController>();
        }
        public override void Enter()
        {
            base.Enter();

            float verticalForce;
            float planarForce;
            JumpType jumpType = JumpType.Slip;
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
            controller.Jump(jumpType, verticalForce, planarForce);
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Input.CrouchPressed && _character.Input.MovementInput.magnitude > 0;
        }

    }

}
