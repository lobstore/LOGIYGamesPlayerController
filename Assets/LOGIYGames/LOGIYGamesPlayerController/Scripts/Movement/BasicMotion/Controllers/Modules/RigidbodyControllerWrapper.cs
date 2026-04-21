using UnityEngine;

namespace LOGIYGames
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyControllerWrapper : ControllerWrapperBase
    {
        [Header("Rigidbody Controller Settings")]
        [SerializeField] private float groundDrag;
        [SerializeField] private float airDrag;


        [Header("Movement Settings")]
        [Tooltip("Force mode for movement (ignored if useAddForce is false)")]
        [SerializeField] private ForceMode m_movementForceMode = ForceMode.Acceleration;
        [Tooltip("Movement force multiplier")]
        [SerializeField] private float m_movementForceMultiplier = 10f;

        private Rigidbody m_rigidbody;
        private CapsuleCollider m_capsuleCollider;
        private SensorsModule m_sensors;

        private float m_Height;
        private float m_Radius;
        private Vector3 m_Center;

        #region Public Properties

        public override float MaxStepHeight { get; set; }
        public override float Height
        {
            get => m_Height;
            set
            {
                m_Height = value;
                UpdateCapsuleDimensions();
            }
        }

        public override float SlopeLimit { get; set; }

        public override Vector3 Center
        {
            get => m_Center;
            set
            {
                m_Center = value;
                UpdateCapsuleCenter();
            }
        }

        public override float Radius
        {
            get => m_Radius;
            set
            {
                m_Radius = value;
                UpdateCapsuleDimensions();
            }
        }

        public override Vector3 Position => m_rigidbody.position;
        public override Quaternion Rotation => m_rigidbody.rotation;
        public override Transform Transform => m_rigidbody.transform;

        public override bool UseGravity { get => m_rigidbody.useGravity; set => m_rigidbody.useGravity = value; }

        public override Vector3 Velocity => m_rigidbody.linearVelocity;
        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_capsuleCollider = GetComponent<CapsuleCollider>();
            m_sensors = GetComponent<SensorsModule>();
            Debug.Assert(m_rigidbody != null, "Error (RigidbodyControllerWrapper): Could not find Rigidbody component");
            Debug.Assert(m_capsuleCollider != null, "Error (RigidbodyControllerWrapper): Could not find CapsuleCollider component");

            // Configure Rigidbody for character controller
            m_rigidbody.freezeRotation = true;
            m_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            m_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Cache initial capsule values
            m_Radius = m_capsuleCollider.radius;
            m_Height = m_capsuleCollider.height;
            m_Center = m_capsuleCollider.center;
        }

        private void Update()
        {
            if (m_sensors.IsGrounded)
            {
                m_rigidbody.linearDamping = groundDrag;
            }
            else
            {
                m_rigidbody.linearDamping = airDrag;
            }
            m_rigidbody.useGravity = !m_sensors.IsOnSlope;

        }
        #endregion

        #region Movement Methods

        public override void Move(Vector3 a_move)
        {
            Vector3 horizontalVelocity = Vector3.zero;
            Vector3 force = Vector3.zero;
            horizontalVelocity = m_rigidbody.linearVelocity;
            horizontalVelocity.y = 0;
            if (m_sensors.IsGrounded)
            {
                force = a_move;
                if (m_sensors.IsOnSlope)
                {
                    if (UseProjectionOnPlane)
                    {
                        force = Vector3.ProjectOnPlane(force, m_sensors.BelowHit.normal);
                    }
                }
                m_rigidbody.AddForce(force - horizontalVelocity, m_movementForceMode);
            }
            else
            {
                force = a_move;
                m_rigidbody.AddForce(force - horizontalVelocity, ForceMode.Force);
            }


        }
        public override void ResetVelocity()
        {
            m_rigidbody.linearVelocity = Vector3.zero;
        }
        public override void SetRotation(Quaternion a_targetRotation)
        {
            m_rigidbody.MoveRotation(a_targetRotation);
            m_rigidbody.PublishTransform();
        }


        #endregion

        #region Transform Methods

        public override void SetPosition(Vector3 a_position)
        {
            m_rigidbody.position = a_position;
        }


        #endregion

        #region Jump Method

        public override void Jump(Vector3 force)
        {
            m_rigidbody.linearVelocity = new Vector3(m_rigidbody.linearVelocity.x, 0, m_rigidbody.linearVelocity.z);
            m_rigidbody.AddForce(force, ForceMode.Impulse);
        }

        #endregion

        #region Capsule Management

        private void UpdateCapsuleDimensions()
        {
            if (m_capsuleCollider == null) return;

            m_capsuleCollider.radius = m_Radius;
            m_capsuleCollider.height = m_Height;
        }

        private void UpdateCapsuleCenter()
        {
            if (m_capsuleCollider == null) return;

            m_capsuleCollider.center = m_Center;
        }


        #endregion


    }
}
