using System;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class MovementData
    {
        public float Acceleration;
        public float Deceleration;
        public float TargetSpeed;
        public float CurrentSpeed;
        public float TurnSmoothTime;
        public Quaternion TargetRotation;
        public Vector3 TargetDirection;
        public float DeltaYaw;
    }

}
