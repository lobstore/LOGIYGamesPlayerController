using System;

namespace LOGIYGames
{
    [Serializable]
    public class MovementStateData
    {
        public float TurnSmoothTime = 8;
        public float Acceleration = 6;
        public float Deceleration = 6;
        public float Speed = 0;
        public float ActionFrameDuration = 0;

        public bool IsAnimationDrivenMovement;
        public bool IsAnimationDrivenRotation;
        public bool UseProjectionOnPlane;
    }
}
