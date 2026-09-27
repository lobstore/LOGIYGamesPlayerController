using System;

namespace LOGIYGames
{
    [Serializable]
    public class MovementStateData
    {
        public float TurnSmoothTime = 8;

        public float Acceleration = 4;
        public float Deceleration = 4;

        public float TargetSpeed = 1;
        public float ActionFrameDuration = 0;

        public bool IsAnimationDrivenMovement;
        public bool IsAnimationDrivenRotation;
        public bool UseProjectionOnPlane;
        public bool ResetVelocityOnEnter;
        public bool ResetVelocityOnExit;
        public bool ResetSpeedOnEnter;
        public bool ResetSpeedOnExit;
    }
}
