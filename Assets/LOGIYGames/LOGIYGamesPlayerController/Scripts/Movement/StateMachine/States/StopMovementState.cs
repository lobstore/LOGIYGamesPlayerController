using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;

namespace LOGIYGames.Movement
{
    public class StopMovementState : TimedMovementState
    {
        public StopMovementState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData) { }
        public override void Enter()
        {
            base.Enter();
            Direction dir = _character.GetRelativeMovementDirection();

            _character.EventBus.Publish(new MovementStoppedEvent
            {
                direction = dir,
                speed = _character.SpeedMultiplier
            });
        }
        protected override void Move()
        {
            
        }
        protected override void Rotate()
        {
            
        }
    }

}
