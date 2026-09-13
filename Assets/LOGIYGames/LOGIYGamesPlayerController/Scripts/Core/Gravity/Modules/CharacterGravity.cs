using Alchemy.Inspector;
using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public class CharacterGravity
    {

        public bool useGravity = true;
        public float groundMagnit;

        public Vector3 GravityDirection { get => gravityDirection.normalized; set => gravityDirection = value; }
        public Vector3 gravityDirection = new Vector3(0, -1, 0);

        public float MaxGravityForce = 9.84f;
        public Vector3 CurrentGravity;

        public bool UseGravity { get => useGravity; set => useGravity = value; }

        public SensorsModule m_sensors;
        public float GravityAcceleration;

        [ReadOnly] [SerializeField] private float currentGravityMultiplier;

        public float CurrentGravityMultiplier => currentGravityMultiplier;

        public void Update()
        {

            if (!useGravity)
            {
                currentGravityMultiplier = 0;
                CurrentGravity = Vector3.zero;
                return;
            }

            if (m_sensors != null &&
                m_sensors.IsGrounded &&
                CurrentGravity.y < 0 &&
                m_sensors.IsValidSlope())
            {
                currentGravityMultiplier = groundMagnit;
            }
            else
            {
                currentGravityMultiplier = MaxGravityForce;
            }
            CurrentGravity = Vector3.MoveTowards(CurrentGravity, currentGravityMultiplier * gravityDirection.normalized, Time.deltaTime * GravityAcceleration);

            if (m_sensors != null && m_sensors.AboveHit.collider != null)
            {
                CurrentGravity = GravityDirection.normalized * 0.5f;
            }

        }
    }
}
