using System;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class MovementRuntimeData
    {
        public float BaseSpeed;
        public AccelerationData AccelerationData;
        public float Speed;
        public float CurrentSpeed => Speed * BaseSpeed;
        public float TurnSmoothTime;
        public Quaternion TargetRotation;
        public Vector3 TargetDirection;
        public Vector3 TargetVelocity;
        public float DeltaYaw;
    }

}
