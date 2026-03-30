using LOGIYGames.CharacterCore;
using Unity.VisualScripting;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Wrapper for Rigidbody-based character controller.
    /// Implements the GenericControllerWrapper interface to allow seamless swapping
    /// with UnityControllerWrapper and KinematicControllerWrapper.
    /// Uses Rigidbody.MovePosition and Rigidbody.MoveRotation for physics-based movement.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyControllerWrapper : ControllerWrapperBase
    {
        [Header("Rigidbody Controller Settings")]
        [SerializeField] private bool m_applyGravityWhenGrounded = false;
        [SerializeField] private float groundDrag;
        [SerializeField] private float airDrag;


        [Header("Movement Settings")]
        [Tooltip("Force mode for movement (ignored if useAddForce is false)")]
        [SerializeField] private ForceMode m_movementForceMode = ForceMode.Acceleration;
        [Tooltip("Movement force multiplier")]
        [SerializeField] private float m_movementForceMultiplier = 10f;
        
        private Rigidbody m_rigidbody;
        private CapsuleCollider m_capsuleCollider;
        private CharacterGravityModule m_characterGravityModule;
        private SensorsModule m_sensors;
        private Character m_character;

        private bool m_collisionEnabled = true;

        // Cached values for properties
        private float m_cachedHeight;
        private float m_cachedRadius;
        private Vector3 m_cachedCenter;
        
        #region Public Properties

        public override bool IsGrounded => m_sensors != null && m_sensors.IsGrounded;

        public override Vector3 Velocity => m_rigidbody != null ? m_rigidbody.linearVelocity : Vector3.zero;
        
        public override bool CollisionEnabled
        {
            get => m_collisionEnabled;
            set
            {
                m_collisionEnabled = value;
                m_capsuleCollider.enabled = value;
            }
        }
        
        public override float MaxStepHeight { get; set; }  
        public override float Height
        {
            get => m_cachedHeight;
            set
            {
                m_cachedHeight = value;
                UpdateCapsuleDimensions();
            }
        }
        
        public override float SlopeLimit {  get; set; }
        
        public override Vector3 Center
        {
            get => m_cachedCenter;
            set
            {
                m_cachedCenter = value;
                UpdateCapsuleCenter();
            }
        }
        
        public override float Radius
        {
            get => m_cachedRadius;
            set
            {
                m_cachedRadius = value;
                UpdateCapsuleDimensions();
            }
        }
        
        public override bool ApplyGravityWhenGrounded => m_applyGravityWhenGrounded;
        
        #endregion
        
        #region Unity Lifecycle

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_capsuleCollider = GetComponent<CapsuleCollider>();
            m_characterGravityModule = GetComponent<CharacterGravityModule>();
            m_sensors = GetComponent<SensorsModule>();
            m_character = GetComponent<Character>();
            Debug.Assert(m_rigidbody != null, "Error (RigidbodyControllerWrapper): Could not find Rigidbody component");
            Debug.Assert(m_capsuleCollider != null, "Error (RigidbodyControllerWrapper): Could not find CapsuleCollider component");

            // Configure Rigidbody for character controller
            m_rigidbody.freezeRotation = true;
            m_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            m_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Cache initial capsule values
            m_cachedRadius = m_capsuleCollider.radius;
            m_cachedHeight = m_capsuleCollider.height;
            m_cachedCenter = m_capsuleCollider.center;
        }

        private void Update()
        {
            if (IsGrounded)
            {
                m_rigidbody.linearDamping = groundDrag;
            }
            else
            {
                m_rigidbody.linearDamping = airDrag;
            }
        }
        #endregion

        #region Movement Methods

        public override void Move(Vector3 a_move)
        {
     
            if (m_collisionEnabled)
            {
                if (IsGrounded)
                {

                    a_move = Vector3.ProjectOnPlane(a_move, m_sensors.BelowHit.normal);
    
                }

                if (m_rigidbody.linearVelocity.magnitude <= m_character.CurrentSpeed)
                {
                    Vector3 force = a_move * m_movementForceMultiplier;
                    m_rigidbody.AddForce(force, m_movementForceMode);
                }
                else
                {

                    m_rigidbody.linearVelocity = new Vector3(a_move.x, m_rigidbody.linearVelocity.y, a_move.z);

                }
            }
            else
            {
                m_rigidbody.position += a_move * Time.fixedDeltaTime;
            }
        }

        public override void Rotate(Quaternion a_targetRotation)
        {
            m_rigidbody.MoveRotation(a_targetRotation);
        }
        

        #endregion
        
        #region Transform Methods
        
        public override void SetPosition(Vector3 a_position)
        {
            m_rigidbody.position = a_position;
        }
       
        
        public override void SetPositionAndRotation(Vector3 a_position, Quaternion a_rotation)
        {
            m_rigidbody.position = a_position;
            m_rigidbody.rotation = a_rotation;
        }
        
        public override Vector3 GetCachedMoveDelta()
        {
            return m_rigidbody.linearVelocity*Time.deltaTime;
        }
        
        public override Quaternion GetCachedRotDelta()
        {
            return m_rigidbody.rotation;
        }
        
        #endregion
        
        #region Jump Method
        
        public override void Jump(float force)
        {
            m_rigidbody.linearVelocity = new Vector3(m_rigidbody.linearVelocity.x, 0, m_rigidbody.linearVelocity.z);
            m_rigidbody.AddForce(Vector3.up * Mathf.Sqrt(force * -2f * Physics.gravity.y), ForceMode.Impulse);
        }
        
        #endregion
        
        #region Capsule Management
        
        private void UpdateCapsuleDimensions()
        {
            if (m_capsuleCollider == null) return;
            
            m_capsuleCollider.radius = m_cachedRadius;
            m_capsuleCollider.height = m_cachedHeight;
        }
        
        private void UpdateCapsuleCenter()
        {
            if (m_capsuleCollider == null) return;
            
            m_capsuleCollider.center = m_cachedCenter;
        }
        
        public override Collider GetCollider()
        {
            return m_capsuleCollider;
        }
        
        #endregion
        
        #region Initialization
        
        public override void Initialize()
        {
            // Rigidbody is already configured in Awake
        }
        
        #endregion
        
    }
}
