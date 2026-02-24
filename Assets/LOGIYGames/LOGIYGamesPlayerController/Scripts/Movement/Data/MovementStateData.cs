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
    }
}
