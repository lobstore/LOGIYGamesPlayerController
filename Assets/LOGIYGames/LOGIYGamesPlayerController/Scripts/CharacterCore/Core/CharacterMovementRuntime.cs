using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public sealed class CharacterMovementRuntime
    {
        public CharacterVelocityData Velocity;

        public AccelerationData Acceleration;

        public Vector3 TargetDirection;

        public float Speed;

        public int JumpCount;

        public MovementMode Mode;
    }
}
