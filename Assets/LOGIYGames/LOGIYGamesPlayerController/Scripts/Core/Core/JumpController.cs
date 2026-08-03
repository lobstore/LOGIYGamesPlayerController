using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
namespace LOGIYGames.CharacterCore
{
    public class JumpController
    {
        public int JumpCount;
        public int MaxJumpCount = 2;
        Character Character;
        StaminaController Stamina;
        public JumpController(Character character)
        {
            Character = character;
            Stamina = character.StaminaController;
            EventsSubscription();
        }
        public bool CanExecute(JumpStateData jumpStateData)
        {
            return JumpCount < MaxJumpCount && Stamina.TryUse(jumpStateData.StaminaUsage);
        }
        private void EventsSubscription()
        {
            Character.EventBus.Subscribe<JumpPerformedEvent>((evt) =>
            {
                switch (evt.jumpType)
                {
                    case JumpType.GroundJump:
                        Character.Jump(Character.RuntimeMovement.TargetDirection * evt.planarForce + Character.transform.up * evt.verticalForce);
                        break;
                    case JumpType.HangJump:
                        Character.Jump(Character.Sensors.LegsFrontHit.normal * evt.planarForce + evt.verticalForce * Character.transform.up);
                        break;
                    case JumpType.WallRunJump:
                        break;
                    case JumpType.Roll:
                        Character.Jump(Character.transform.forward * evt.planarForce + Character.transform.up * evt.verticalForce);
                        break;
                    case JumpType.Dash:
                        Character.Jump(Character.RuntimeMovement.TargetDirection * evt.planarForce + Character.transform.up * evt.verticalForce);
                        break;
                    case JumpType.Slip:
                        Character.Jump(Character.RuntimeMovement.TargetDirection * evt.planarForce);
                        break;
                    default:
                        break;
                }
                JumpCount++;
            });
        }
    }
}
