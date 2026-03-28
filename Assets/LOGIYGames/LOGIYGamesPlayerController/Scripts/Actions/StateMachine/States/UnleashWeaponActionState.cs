using LOGIYGames.CharacterCore;

namespace LOGIYGames
{
    public class UnleashWeaponActionState : ActionBaseState
    {
        public UnleashWeaponActionState(Character character) : base(character)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Character.EventBus.Publish(
                new LeashWeaponEvent
                {
                    unleashWeapon = true
                }
            );
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();

        }
    }
}
