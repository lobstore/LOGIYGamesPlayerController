using LOGIYGames.Shared.Enums;

namespace LOGIYGames.Shared.Character.Events
{
    public class JumpPerformedEvent
    {
        public JumpType jumpType;
        public Direction direction;
        public float verticalForce;
        public float planarForce;
    }
    public class DashPerformedEvent : JumpPerformedEvent
    {


    }
    public class RollPerformedEvent : JumpPerformedEvent
    {

    }
    public class SlipPerformedEvent : JumpPerformedEvent
    {
    }
    public class TurnPerformedEvent
    {
        public float movementSpeed;
        public float angle;
    }
    public class BackTurnPerformedEvent
    {
        public float movementSpeed;
        public float angle;
    }
    public class LeashWeaponEvent
    {
        public bool unleashWeapon;
    }
    public class ItemThrowedEvent
    {

    }
    public class LandedEvent
    {
        public Direction horizontalDirection;
        public float fallingSpeed;
    }
    public class LadderEnteredEvent
    {
        public Direction from;
    }
    public class LadderExitedEvent
    {
        public Direction from;
    }
    public class MovementStoppedEvent
    {
        public Direction direction;
        public float speed;
    }
    public partial class MantlingEvent
    {
        public MantlingType Type;
    }
}