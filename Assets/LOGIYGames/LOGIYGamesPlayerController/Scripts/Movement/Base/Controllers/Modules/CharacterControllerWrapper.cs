using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public struct GroundedReport
    {
        public Vector3 GroundedVelocity;
    }

    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerWrapper : MovementWrapperBase
    {
        #region Unity Controller
        [SerializeField]
        private CharacterController m_characterController;
        private CharacterGravityModule m_characterGravityModule;
        private CharacterModule m_character;
        private SensorsModule m_sensors;

        private Vector3 totalVelocity;
        private Vector3 planarVelocity;
        private Vector3 verticalVelocity;

        [SerializeField] private float projectingPlanarVelocityMultiplier;
        [SerializeField] private float slopeSlideMaxSpeed;
        [SerializeField] private float slopeSlideAcceleration;

        GroundedReport lastGroundedReport;
        public override GroundedReport LastGroundedReport => lastGroundedReport;

        #endregion

        #region Ground Motion System

        [Header("Ground Motion")]

        [SerializeField] private bool useGroundMotion = true;

        private Transform currentGroundTransform;

        private Vector3 lastGroundPosition;
        private Quaternion lastGroundRotation;

        private Vector3 groundDeltaPosition;
        private Quaternion groundDeltaRotation;

        #endregion

        #region Properties

        private LayerMask excludeLayers;
        private LayerMask includeLayers;

        public override Collider Collider => m_characterController;

        public override bool IsNoClip
        {
            set
            {
                if (value)
                    m_characterController.excludeLayers = Physics.AllLayers;
                else
                    m_characterController.excludeLayers = excludeLayers;
            }
        }

        public override float MaxStepHeight
        {
            get => m_characterController.stepOffset;
            set => m_characterController.stepOffset = value;
        }

        public override float Height
        {
            get => m_characterController.height;
            set => m_characterController.height = value;
        }

        public override float SlopeLimit
        {
            get => m_characterController.slopeLimit;
            set => m_characterController.slopeLimit = Mathf.Max(0, value);
        }

        public override Vector3 Center
        {
            get => m_characterController.center;
            set => m_characterController.center = value;
        }

        public override float Radius
        {
            get => m_characterController.radius;
            set => m_characterController.radius = value;
        }

        public override bool UseGravity
        {
            get => m_characterGravityModule.UseGravity;
            set => m_characterGravityModule.UseGravity = value;
        }

        public override Vector3 Velocity => m_characterController.velocity;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            m_sensors = GetComponent<SensorsModule>();
            m_character = GetComponent<CharacterModule>();
            if (m_characterController == null)
            m_characterController = GetComponent<CharacterController>();
            m_characterGravityModule = GetComponent<CharacterGravityModule>();

            if (m_characterController == null)
                m_characterController = gameObject.AddComponent<CharacterController>();

            m_characterController.enableOverlapRecovery = true;

            m_sensors.GroundedEvent.AddListener(grounded =>
            {
                if (grounded)
                {
                    lastGroundedReport = new GroundedReport
                    {
                        GroundedVelocity = m_characterController.velocity
                    };
                }
            });

            excludeLayers = m_characterController.excludeLayers;
            includeLayers = m_characterController.includeLayers;
        }

        private void Update()
        {
            verticalVelocity = m_character.VelocityData.Gravity;

            UpdateGroundMotion();
            ApplyGroundMotion();

        }

        #endregion

        #region Movement

        public override void Move(Vector3 a_move)
        {
            planarVelocity = a_move;

            if (!m_sensors.IsValidSlope() &&
                verticalVelocity.y < 0 &&
                UseProjectionOnPlane)
            {
                totalVelocity =
                    Vector3.Lerp(
                        totalVelocity,
                        Vector3.ProjectOnPlane(
                            Vector3.ClampMagnitude(verticalVelocity, slopeSlideMaxSpeed),
                            m_sensors.BelowHit.normal),
                        Time.deltaTime * slopeSlideAcceleration);
            }
            else
            {
                totalVelocity = planarVelocity + verticalVelocity;
            }

            if (m_sensors.IsOnSlope && UseProjectionOnPlane)
            {
                ProjectVelocity();
            }

            if (m_characterController == null || !m_characterController.enabled)
            {
                transform.Translate(totalVelocity * Time.deltaTime);
                return;
            }

            m_characterController.Move(totalVelocity * Time.deltaTime);
        }

        public override void ForceMove(Vector3 a_move)
        {
            m_characterController.Move(a_move * Time.deltaTime);
        }

        private void ProjectVelocity()
        {
            Vector3 projectedPlanarVelocity =
                Vector3.ProjectOnPlane(planarVelocity, m_sensors.BelowHit.normal);

            planarVelocity =
                Vector3.Lerp(planarVelocity, projectedPlanarVelocity,
                    Time.deltaTime * projectingPlanarVelocityMultiplier);
        }

        public override void SetRotation(Quaternion a_targetRotation)
        {
            m_characterController.transform.rotation = a_targetRotation;
        }

        public override void SetPosition(Vector3 a_position)
        {
            transform.position = a_position;
        }

        public override void AddForce(Vector3 force) { }

        public override void ResetVelocity()
        {
            totalVelocity = Vector3.zero;
            planarVelocity = Vector3.zero;
            verticalVelocity = Vector3.zero;
        }

        #endregion

        #region Ground Motion

        private void UpdateGroundMotion()
        {
            if (!useGroundMotion)
                return;

            if (!m_sensors.IsGrounded || m_sensors.BelowHit.collider == null)
            {
                currentGroundTransform = null;
                groundDeltaPosition = Vector3.zero;
                groundDeltaRotation = Quaternion.identity;
                return;
            }

            Transform newGround =
                m_sensors.BelowHit.collider.transform;

            if (currentGroundTransform != newGround)
            {
                currentGroundTransform = newGround;

                lastGroundPosition = currentGroundTransform.position;
                lastGroundRotation = currentGroundTransform.rotation;

                groundDeltaPosition = Vector3.zero;
                groundDeltaRotation = Quaternion.identity;

                return;
            }

            groundDeltaPosition =
                currentGroundTransform.position - lastGroundPosition;

            groundDeltaRotation =
                currentGroundTransform.rotation *
                Quaternion.Inverse(lastGroundRotation);

            lastGroundPosition = currentGroundTransform.position;
            lastGroundRotation = currentGroundTransform.rotation;
        }

        private void ApplyGroundMotion()
        {
            if (!useGroundMotion) return;
            if (currentGroundTransform == null) return;

            if (groundDeltaPosition != Vector3.zero)
            {
                m_characterController.Move(groundDeltaPosition);
            }

            // Оставляем только вращение вокруг оси Y (yaw)
            if (groundDeltaRotation != Quaternion.identity)
            {
                float yaw = groundDeltaRotation.eulerAngles.y;
                Quaternion yRotation = Quaternion.Euler(0f, yaw, 0f);

                Vector3 localOffset = transform.position - currentGroundTransform.position;
                localOffset = yRotation * localOffset;

                Vector3 rotatedPosition = currentGroundTransform.position + localOffset;
                Vector3 delta = rotatedPosition - transform.position;

                m_characterController.Move(delta);

                // применяем только Y-вращение к персонажу
                Vector3 currentEuler = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y + yaw, currentEuler.z);
            }
        }

        #endregion
    }
}