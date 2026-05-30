using LOGIYGames.Shared.Data;
using LOGIYGames.Shared.Enums;
using System;
using System.Collections.Generic;

namespace LOGIYGames.Shared.Character.Events
{
    public abstract class EventBase { }
    [Serializable]
    public class JumpPerformedEvent: EventBase
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
    public class BackTurnPerformedEvent : EventBase
    {
        public float movementSpeed;
        public float angle;
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
        public float speed;
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
        public DamageData DamageData {  get; set; }
    }
    [Serializable]
    public class SkillUsedEvent : EventBase
    {
       public Ability AbilityData { get; set; }
    }
    [Serializable]
    public class ComboAnimationEvent : EventBase
    {
       public ComboEventType ComboEventType { get; set; }
    }
    [Serializable]
    public class TimedEvent : EventBase
    {
        public float EventTime;
    }
    [Serializable]
    public class AbilityTimedEvent : TimedEvent
    {
        public AbilityEventType AbilityEventType;

        public string animationName;
        public float CrossFade = 0.1f;
        public float MotionSpeed = 1f;

        public bool UseRootMotion = true;
        public List<EffectFactory> effects = new();
    }
}