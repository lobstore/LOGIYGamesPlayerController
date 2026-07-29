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
    public class LandedEvent : EventBase
    {
        public Direction horizontalDirection;
        public float fallingSpeed;
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
    public class AnimationEvent : EventBase
    {
        public AnimationData AnimationData;
    }
}