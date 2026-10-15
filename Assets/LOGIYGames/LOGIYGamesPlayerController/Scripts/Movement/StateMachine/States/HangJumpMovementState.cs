using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;

namespace LOGIYGames.Movement
{
    public class HangJumpMovementState : TimedMovementState
    {
        private JumpStateData _jumpStateData;
        JumpController controller;
        public HangJumpMovementState(Actor ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _jumpStateData = stateData;
            controller = ctx.GetComponent<JumpController>();
        }

        public override void Enter()
        {
            base.Enter();

            float verticalForce;
            float planarForce;
            JumpType jumpType = JumpType.HangJump;
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
            return base.CanEnter() && _character.Input.JumpPressed;
        }
    }
}
