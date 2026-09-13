using UnityEngine;

namespace LOGIYGames
{

    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerWrapper : MovementWrapperBase
    {
        [SerializeField]
        private CharacterController m_characterController;
        private SensorsModule m_sensors;

        private Vector3 linearVelocity;

        GroundedReport lastGroundedReport;
        public override GroundedReport LastGroundedReport => lastGroundedReport;


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
        [SerializeField] private float damping;

        [SerializeField] private float aerialDamping;
        [SerializeField] private float groundDamping;
        [SerializeField] private bool useGravity;
        [SerializeField] bool freeze;
        public bool Freeze {  get; set; }
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
            get => useGravity;
            set => useGravity = value;
        }

        public override Vector3 Velocity => linearVelocity;


        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            m_sensors = GetComponent<SensorsModule>();
            if (m_characterController == null)
                m_characterController = GetComponent<CharacterController>();

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
            if (freeze) return;
            if (useGravity)
            {
                AddAcceleration(Physics.gravity);
                if (m_sensors.IsGrounded && linearVelocity.y < 0)
                {
                    linearVelocity.y = -1f;
                }
            }

            damping = m_sensors.IsGrounded ? groundDamping : aerialDamping;
            linearVelocity = Vector3.MoveTowards(linearVelocity, Vector3.zero, damping * Time.deltaTime);

            UpdateGroundMotion();
            ApplyGroundMotion();
            ProjectVelocity();
            m_characterController.Move(linearVelocity * Time.deltaTime);
        }
        #endregion

        #region Movement

        public override void ChangeVelocity(Vector3 Velocity)
        {
            linearVelocity = Velocity;
        }
        public override void AddAcceleration(Vector3 accelerationForce)
        {
            linearVelocity += accelerationForce * Time.deltaTime;
        }
        public override void ForceMove(Vector3 velocity)
        {
            linearVelocity = velocity;
        }

        public override void SetRotation(Quaternion a_targetRotation)
        {
            m_characterController.transform.rotation = a_targetRotation;
        }

        public override void SetPosition(Vector3 a_position)
        {
            transform.position = a_position;
        }

        public override void AddImpulse(Vector3 impulseForce)
        {
            // m_characterGravityModule.CurrentGravity.y += impulseForce.y;
            linearVelocity += impulseForce;
        }

        public override void ResetVelocity()
        {
            linearVelocity = Vector3.zero;
        }

        public override void ResetGravity()
        {
            linearVelocity.y = 0;
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

        private void ProjectVelocity()
        {
            if (m_sensors.IsOnSlope && linearVelocity.y<0)
            {
                Vector3 projectedVelocity =
                    Vector3.ProjectOnPlane(linearVelocity, m_sensors.BelowHit.normal);

                linearVelocity = projectedVelocity;
            }
        }

        public override void DisableMovement()
        {
            freeze = true;
            useGravity = false;
        }

        public override void EnableMovement()
        {
            freeze = false;
            useGravity = true;
        }
    }
}