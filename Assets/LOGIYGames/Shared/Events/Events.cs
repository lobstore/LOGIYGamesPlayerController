using LOGIYGames.Shared.Enums;

namespace LOGIYGames.Shared.Character.Events
{
    public class JumpPerformedEvent
    {

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
    public class TurnPerformedEvent
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

    }
    public class LadderEnteredEvent
    {
        public Direction from;
    }
    public class LadderExitedEvent
    {
        public Direction from;
    }
}