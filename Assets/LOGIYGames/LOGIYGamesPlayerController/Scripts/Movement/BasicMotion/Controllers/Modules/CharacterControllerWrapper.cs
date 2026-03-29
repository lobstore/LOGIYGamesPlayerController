using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Wrapper for Unity's built-in CharacterController.
    /// Implements the GenericControllerWrapper interface to allow seamless swapping
    /// with KinematicControllerWrapper.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerWrapper : ControllerWrapperBase
    {
        [Header("Unity Controller Settings")]
        [SerializeField]
        private bool m_applyGravityWhenGrounded = false;

        private CharacterController m_characterController;
        private CharacterGravityModule m_characterGravityModule;
        private Character m_character;
        private SensorsModule m_sensors;

        private bool m_enableCollision = true;
        private Vector3 totalVelocity;
        private Vector3 totalVerticalVelocity;
        private Vector3 totalPlanarVelocity;
        Vector3 planarVelocity;
        Vector3 verticalVelocity;
        [SerializeField] private float projectingPlanarSpeed;
        [SerializeField] private float projectingVerticalSpeed;

        #region Public Properties

        public override bool IsGrounded { get { return m_sensors.IsGrounded; } }

        public override bool ApplyGravityWhenGrounded { get { return m_applyGravityWhenGrounded; } }

        public override Vector3 Velocity { get { return m_characterController.velocity; } }

        public override float MaxStepHeight
        {
            get { return m_characterController.stepOffset; }
            set { m_characterController.stepOffset = value; }
        }

        public override float Height
        {
            get { return m_characterController.height; }
            set { m_characterController.height = value; }
        }

        public override float SlopeLimit
        {
            get => m_characterController.slopeLimit;
            set => m_characterController.slopeLimit = Mathf.Max(0, value);
        }

        public override Vector3 Center
        {
            get { return m_characterController.center; }
            set { m_characterController.center = value; }
        }

        public override float Radius
        {
            get { return m_characterController.radius; }
            set { m_characterController.radius = value; }
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            m_sensors = GetComponent<SensorsModule>();
            m_character = GetComponent<Character>();
            m_characterController = GetComponent<CharacterController>();
            if (m_characterController == null)
            {
                m_characterController = gameObject.AddComponent<CharacterController>();
            }
            m_characterGravityModule = GetComponent<CharacterGravityModule>();

            m_characterController.enableOverlapRecovery = true;
        }

        private void LateUpdate()
        {
            DebugDraw.DrawArrow(transform.position, totalVelocity, Color.green);
        }

        #endregion

        #region Movement Methods

        public override void Move(Vector3 a_move)
        {
            planarVelocity = a_move;
            verticalVelocity = m_characterGravityModule.Velocity;


            if (m_sensors.IsValidSlope())
            {
                if (m_sensors.IsGrounded && m_characterGravityModule.Velocity.y < 0 && m_character.Input.MovementInput.magnitude> 0)
                {
                    ProjectVelocity();
                }
                else
                {
                    totalPlanarVelocity = planarVelocity;
                    totalVerticalVelocity = verticalVelocity;
                }

            }
            else
            {
                ProjectVelocity();
            }



            totalVelocity = totalVerticalVelocity + totalPlanarVelocity;

            if (m_enableCollision)
            {
                m_characterController.Move(totalVelocity * Time.deltaTime);
            }
            else
            {

                m_characterController.transform.Translate(totalVelocity * Time.deltaTime, Space.World);
            }
        }

        private void ProjectVelocity()
        {
            Vector3 projectedPlanarVelocity = Vector3.zero;
            Vector3 projectedVerticalVelocity = Vector3.zero;
            projectedPlanarVelocity = Vector3.ProjectOnPlane(planarVelocity, m_sensors.BelowHit.normal);
            projectedVerticalVelocity = Vector3.ProjectOnPlane(verticalVelocity, m_sensors.BelowHit.normal);
            totalPlanarVelocity = Vector3.Lerp(totalPlanarVelocity, projectedPlanarVelocity, Time.deltaTime * projectingPlanarSpeed);
            totalVerticalVelocity = Vector3.Lerp(totalVerticalVelocity, projectedVerticalVelocity, Time.deltaTime * projectingVerticalSpeed);
        }

        public override void Rotate(Quaternion a_targetRotation)
        {
            m_characterController.transform.rotation = a_targetRotation;
        }

        #endregion

        #region Transform Methods

        public override void SetPosition(Vector3 a_position)
        {
            transform.position = a_position;
        }


        public override void SetPositionAndRotation(Vector3 a_position, Quaternion a_rotation)
        {
            transform.SetPositionAndRotation(a_position, a_rotation);
        }

        public override Vector3 GetCachedMoveDelta()
        {
            return Velocity * Time.deltaTime;
        }

        public override Quaternion GetCachedRotDelta()
        {
            return transform.rotation;
        }

        #endregion

        #region Collision Management

        public override bool CollisionEnabled
        {
            get { return m_enableCollision; }
            set
            {
                m_enableCollision = value;
            }
        }

        #endregion

        #region Jump Method

        public override void Jump(float force)
        {
            if (m_characterGravityModule != null)
            {
                m_characterGravityModule.Velocity = transform.up * Mathf.Sqrt(force * -2f * Physics.gravity.y);
            }
        }

        #endregion

        #region Initialization

        public override void Initialize()
        {
            m_characterController.enableOverlapRecovery = true;
        }

        #endregion

        #region Collider Access

        public override Collider GetCollider()
        {
            return m_characterController;
        }

        #endregion

        #region Unity Lifecycle Events

        private void OnEnable()
        {
            m_characterController.enabled = true;
        }

        private void OnDisable()
        {
            m_characterController.enabled = false;
        }

        #endregion


    }
}
