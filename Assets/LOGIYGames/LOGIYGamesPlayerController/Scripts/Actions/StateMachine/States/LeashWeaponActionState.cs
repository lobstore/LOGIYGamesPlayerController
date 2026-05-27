using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;

namespace LOGIYGames
{
    public class LeashWeaponActionState : CharacterActionState
    {
        public LeashWeaponActionState(Character character) : base(character)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _character.EventBus.Publish(
                new LeashWeaponEvent
                {
                    unleashWeapon = false
                }
            );
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();

        }
    }
}
