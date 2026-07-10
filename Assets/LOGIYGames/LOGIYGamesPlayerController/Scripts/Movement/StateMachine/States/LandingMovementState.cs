using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using System;

namespace LOGIYGames.Movement
{
    public class LandingMovementState : TimedMovementState
    {
        MovementWrapperBase controller;
        public LandingMovementState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData)
        {
            controller = ctx.GetComponent<MovementWrapperBase>();
        }

        public override void Enter()
        {
            base.Enter();
            _durationTimer.Reset(MathF.Abs(controller.LastGroundedReport.GroundedVelocity.y) / 10);
            Direction dir = _character.GetRelativeMovementDirection();
            _character.EventBus.Publish(new LandedEvent
            {
                horizontalDirection = dir,
                fallingSpeed = controller.LastGroundedReport.GroundedVelocity.y
            });
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.IsGrounded;
        }
    }

}
