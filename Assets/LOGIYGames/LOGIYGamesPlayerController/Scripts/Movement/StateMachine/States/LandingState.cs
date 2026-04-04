using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;

namespace LOGIYGames.Movement
{
    public class LandingState : TimedMovementState
    {
        public LandingState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData) { }

        public override void Enter()
        {
            base.Enter();
            _character.EventBus.Publish(new LandedEvent ());
        }
    }

}
