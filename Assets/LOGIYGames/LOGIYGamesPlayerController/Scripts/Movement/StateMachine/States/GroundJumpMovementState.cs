using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Enums;

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
            controller.Enter();
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
            controller.Jump(jumpType, verticalForce, planarForce);
        }
        public override void Exit()
        {
            base.Exit();
            controller.Exit();
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
