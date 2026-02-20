using UnityEngine;
using UnityEngine.Assertions;

namespace LOGIYGames
{
    /// <summary>
    /// Wrapper for Unity's built-in CharacterController.
    /// Implements the GenericControllerWrapper interface to allow seamless swapping
    /// with KinematicControllerWrapper.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class UnityControllerWrapper : GenericControllerWrapper
    {
        [Header("Unity Controller Settings")]
        [SerializeField]
        private bool m_applyGravityWhenGrounded = false;

        private CharacterController m_characterController;
        private CharacterGravityModule m_characterGravityModule;

        private bool m_enableCollision = true;
        private Vector3 m_cachedMoveDelta = Vector3.zero;
        private Quaternion m_cachedRotDelta = Quaternion.identity;

        #region Public Properties

        public override bool IsGrounded { get { return m_characterController.isGrounded; } }
        
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
            m_characterController = GetComponent<CharacterController>();
            m_characterGravityModule = GetComponent<CharacterGravityModule>();
            Assert.IsNotNull(m_characterController, "Error (UnityControllerWrapper): Could not find CharacterController component");

            m_characterController.enableOverlapRecovery = true;
        }

        #endregion

        #region Movement Methods

        public override void Move(Vector3 a_move)
        {
            if (m_enableCollision)
            {
                m_cachedMoveDelta = a_move * Time.deltaTime;
                m_characterController.Move(m_cachedMoveDelta);
            }
            else
            {
                m_characterController.transform.Translate(a_move * Time.deltaTime, Space.World);
                m_cachedMoveDelta = a_move * Time.deltaTime;
            }
        }

        public override void Rotate(Quaternion a_targetRotation)
        {
            m_characterController.transform.rotation = a_targetRotation;
            m_cachedRotDelta = a_targetRotation * Quaternion.Inverse(transform.rotation);
        }

        #endregion

        #region Transform Methods

        public override void SetPosition(Vector3 a_position)
        {
            transform.position = a_position;
        }

        public override void SetRotation(Quaternion a_rotation)
        {
            transform.rotation = a_rotation;
        }

        public override void SetPositionAndRotation(Vector3 a_position, Quaternion a_rotation)
        {
            transform.SetPositionAndRotation(a_position, a_rotation);
        }

        public override Vector3 GetCachedMoveDelta() => m_cachedMoveDelta;
        
        public override Quaternion GetCachedRotDelta() => m_cachedRotDelta;

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
