using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;

namespace LOGIYGames.Movement
{
    public class LandingMovementState : TimedMovementState
    {
        ControllerWrapperBase controller;
        public LandingMovementState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData) {
            controller = ctx.GetComponent<ControllerWrapperBase>();
        }

        public override void Enter()
        {
            base.Enter();
            Direction dir = _character.GetRelativeMovementDirection();
            _character.EventBus.Publish(new LandedEvent
            {
                horizontalDirection = dir,
                fallingSpeed = controller.Velocity.y
            });
        }
    }

}
