using LOGIYGames.CharacterCore;

namespace LOGIYGames
{
    public class LeashWeaponActionState : ActionBaseState
    {
        public LeashWeaponActionState(Character character) : base(character)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Character.EventBus.Publish(
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
