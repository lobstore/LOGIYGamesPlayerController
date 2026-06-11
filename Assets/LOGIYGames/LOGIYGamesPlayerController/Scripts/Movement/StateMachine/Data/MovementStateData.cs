using System;

namespace LOGIYGames
{
    [Serializable]
    public class MovementStateData
    {
        public float TurnSmoothTime = 8;
        public AccelerationData AccelerationData = new AccelerationData
        {
            Acceleration = 4,
            Deceleration = 4
        };
        public float Speed = 1;
        public float ActionFrameDuration = 0;

        public bool IsAnimationDrivenMovement;
        public bool IsAnimationDrivenRotation;
        public bool UseProjectionOnPlane;
        public bool ResetVelocityOnEnter;
        public bool ResetVelocityOnExit;
        public bool ResetSpeedOnExit;
    }
}
