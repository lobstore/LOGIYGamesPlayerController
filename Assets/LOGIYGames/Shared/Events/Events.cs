using LOGIYGames.Shared.Data;
using LOGIYGames.Shared.Enums;
using System;

namespace LOGIYGames.Shared.Character.Events
{
    public abstract class EventBase { }
    [Serializable]
    public class JumpPerformedEvent : EventBase
    {
        public JumpType jumpType;
        public Direction direction;
        public float verticalForce;
        public float planarForce;
    }
    [Serializable]
    public class TurnPerformedEvent : EventBase
    {
        public float movementSpeed;
        public float angle;
    }
    [Serializable]
    public class BackTurnPerformedEvent : TurnPerformedEvent
    {
    }
    [Serializable]
    public class LeashWeaponEvent : EventBase
    {
        public bool unleashWeapon;
    }
    [Serializable]
    public class ItemThrowedEvent : EventBase
    {

    }
    [Serializable]
    public class LandedEvent : EventBase
    {
        public Direction horizontalDirection;
        public float fallingSpeed;
    }
    [Serializable]
    public class LadderEnteredEvent : EventBase
    {
        public Direction from;
    }
    [Serializable]
    public class LadderExitedEvent : EventBase
    {
        public Direction from;
    }
    [Serializable]
    public class MovementStoppedEvent : EventBase
    {
        public Direction direction;
        public float movementSpeed;
    }
    [Serializable]
    public class MantlingEvent : EventBase
    {
        public MantlingType Type;
    }
    [Serializable]
    public class WallrunEnterEvent : EventBase
    {
        public bool IsRightSide;
    }
    [Serializable]
    public class DamageTakenEvent : EventBase
    {
        public DamageData DamageData { get; set; }
    }
    [Serializable]
    public class SkillUsedEvent : EventBase
    {
        public AbilityFactory AbilityData { get; set; }
    }
    [Serializable]
    public class ComboAnimationEvent : EventBase
    {
        public ComboEventType ComboEventType { get; set; }
    }
    [Serializable]
    public class AnimationEvent : EventBase
    {
        public AnimationData AnimationData;
    }
}