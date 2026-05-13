using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Handles gravity for characters using either Unity CharacterController or KinematicCharacterController.
    /// Works with GenericControllerWrapper for seamless controller swapping.
    /// </summary>
    public class CharacterGravityModule : MonoModuleBase
    {
        [Header("Physics")]
        [SerializeField] bool useGravity;
        [SerializeField] private float groundMagnit;

        public Vector3 GravityDirection { get => gravityDirection.normalized; set => gravityDirection = value; }
        [SerializeField] Vector3 gravityDirection = new Vector3(0, -1, 0);

        public float MaxGravityForce = 9.84f;
        public float CurrentGravityForce;

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
                CurrentGravityForce = 0;
                m_character.VelocityData.Gravity = Vector3.zero;
                return;
            }

            // Check if grounded and on valid slope
            if (m_sensors != null &&
                m_sensors.IsGrounded &&
                m_character.VelocityData.Gravity.y < 0 &&
                m_sensors.IsValidSlope())
            {
                CurrentGravityForce = groundMagnit;
            }
            else
            {
                CurrentGravityForce = MaxGravityForce;
            }
            m_character.VelocityData.Gravity = Vector3.MoveTowards(m_character.VelocityData.Gravity, CurrentGravityForce * gravityDirection.normalized, Time.deltaTime * GravityAcceleration);

            // Check for overhead obstacles
            if (m_sensors != null && m_sensors.AboveHit.collider != null)
            {
                m_character.VelocityData.Gravity = GravityDirection.normalized * 0.5f;
            }
        }
    }
}
