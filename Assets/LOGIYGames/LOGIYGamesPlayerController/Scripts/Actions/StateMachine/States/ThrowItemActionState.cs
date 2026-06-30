using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;

namespace LOGIYGames
{
    public class ThrowItemActionState : CharacterActionState
    {
        public ThrowItemActionState(Character character) : base(character)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _character.EventBus.Publish(new ItemThrowedEvent());
        }
    }
}
