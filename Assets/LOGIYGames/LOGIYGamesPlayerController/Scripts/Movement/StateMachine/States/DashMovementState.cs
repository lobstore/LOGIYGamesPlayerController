using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;

namespace LOGIYGames
{
    public class DashMovementState : TimedMovementState
    {
        private JumpStateData _jumpStateData;
        JumpController controller;
        public DashMovementState(Actor ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _jumpStateData = stateData;
            controller = ctx.GetComponent<JumpController>();
        }
        public override void Enter()
        {
            base.Enter();
            controller.Enter();
            float verticalForce;
            float planarForce;
            JumpType jumpType = JumpType.Dash;
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
            controller.Jump(jumpType, verticalForce, planarForce);

        }
        public override void Exit()
        {
            base.Exit();
            controller.Exit();
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Input.SprintPressing;
        }

    }
}
