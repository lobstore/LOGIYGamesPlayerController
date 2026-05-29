using LOGIYGames.Shared.Data;
using LOGIYGames.Shared.Enums;

namespace LOGIYGames.Shared.Character.Events
{
    public abstract class EventBase { }
    public class JumpPerformedEvent: EventBase
    {
        public JumpType jumpType;
        public Direction direction;
        public float verticalForce;
        public float planarForce;
    }
    public class TurnPerformedEvent : EventBase
    {
        public float movementSpeed;
        public float angle;
    }
    public class BackTurnPerformedEvent : EventBase
    {
        public float movementSpeed;
        public float angle;
    }
    public class LeashWeaponEvent : EventBase
    {
        public bool unleashWeapon;
    }
    public class ItemThrowedEvent : EventBase
    {

    }
    public class LandedEvent : EventBase
    {
        public Direction horizontalDirection;
        public float fallingSpeed;
    }
    public class LadderEnteredEvent : EventBase
    {
        public Direction from;
    }
    public class LadderExitedEvent : EventBase
    {
        public Direction from;
    }
    public class MovementStoppedEvent : EventBase
    {
        public Direction direction;
        public float speed;
    }
    public class MantlingEvent : EventBase
    {
        public MantlingType Type;
    }
    public class WallrunEnterEvent : EventBase
    {
        public bool IsRightSide;
    }
    public class DamageTakenEvent : EventBase
    {
        public DamageData DamageData {  get; set; }
    }
    public class SkillUsedEvent : EventBase
    {
       public Ability AbilityData { get; set; }
    }

    public class CharacterAnimationEvent : EventBase
    {
       public AnimationEventType AnimationEventType { get; set; }
    }
}