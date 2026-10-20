using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;

namespace LOGIYGames.Movement
{
    public class BracedJumpMovementState : TimedMovementState
    {
        private JumpStateData _jumpStateData;
        JumpController controller;
        public BracedJumpMovementState(Actor ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _jumpStateData = stateData;
            controller = ctx.GetComponent<JumpController>();
        }

        public override void Enter()
        {
            base.Enter();
            if (_character.Input.MovementInput .magnitude>0)
            {
                _controller.UseGravity = false;
            }

            controller.Enter();
            float verticalForce;
            float planarForce;
            JumpType jumpType = JumpType.BracedJump;
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
            _controller.UseGravity = true;
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            //controller.Rotate();
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Input.JumpPressed;
        }
    }
}
