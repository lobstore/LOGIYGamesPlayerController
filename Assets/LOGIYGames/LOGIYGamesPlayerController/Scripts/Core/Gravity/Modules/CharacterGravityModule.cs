using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class CharacterGravityModule : MonoModuleBase
    {
        [Header("Physics")]
        [SerializeField] bool useGravity;
        [SerializeField] private float groundMagnit;

        public Vector3 GravityDirection { get => gravityDirection.normalized; set => gravityDirection = value; }
        [SerializeField] Vector3 gravityDirection = new Vector3(0, -1, 0);

        public float MaxGravityForce = 9.84f;
        public float CurrentGravityMultiplier;
        public Vector3 CurrentGravity;

        public bool UseGravity { get => useGravity; set => useGravity = value; }

        [Header("References")]
        [SerializeField] private SensorsModule m_sensors;
        [SerializeField] private Character m_character;
        [SerializeField] private float GravityAcceleration;

        public override void OnFixedUpdate(float fixedDeltaTime)
        {
            base.OnFixedUpdate(fixedDeltaTime);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (!useGravity)
            {
                CurrentGravityMultiplier = 0;
                CurrentGravity = Vector3.zero;
                return;
            }

            if (m_sensors != null &&
                m_sensors.IsGrounded &&
                CurrentGravity.y < 0 &&
                m_sensors.IsValidSlope())
            {
                CurrentGravityMultiplier = groundMagnit;
            }
            else
            {
                CurrentGravityMultiplier = MaxGravityForce;
            }
            CurrentGravity = Vector3.MoveTowards(CurrentGravity, CurrentGravityMultiplier * gravityDirection.normalized, Time.deltaTime * GravityAcceleration);

            if (m_sensors != null && m_sensors.AboveHit.collider != null)
            {
                CurrentGravity = GravityDirection.normalized * 0.5f;
            }
        }
    }
}
