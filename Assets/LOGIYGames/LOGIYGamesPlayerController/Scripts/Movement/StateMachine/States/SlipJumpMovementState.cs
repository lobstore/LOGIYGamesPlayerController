using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames
{
    public class SlipJumpMovementState : TimedMovementState
    {
        private JumpStateData _stateData;
        protected float StandingHeight;
        protected float CrouchHeight;
        public SlipJumpMovementState(Actor ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;
            StandingHeight = _controller.Height;
            CrouchHeight = StandingHeight * 0.5f;
        }
        public override void Enter()
        {
            base.Enter();
            _character.EventBus.Publish(new JumpPerformedEvent
            {
                jumpType = JumpType.Slip,
                planarForce = _stateData.PlanarJumpForce,
            });
        }
        public override void Exit()
        {
            base.Exit();
            _controller.Height = StandingHeight;
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _controller.Height = Mathf.MoveTowards(_controller.Height, CrouchHeight, Time.fixedDeltaTime);
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Input.CrouchPressed && _character.Input.MovementInput.magnitude>0;
        }

    }

}
