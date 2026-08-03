using System;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class MovementRuntimeData
    {
        public float BaseSpeed = 3;
        public AccelerationData AccelerationData { get; set; }
        public float Speed { get; set; }
        public float CurrentSpeed => Speed * BaseSpeed;
        public float TurnSmoothTime { get; set; } = 5f;
        public Quaternion TargetRotation { get; set; }
        public Vector3 TargetDirection { get; set; }
        public Vector3 TargetVelocity { get; set; }
        public float DeltaYaw { get; set; }
    }

}
