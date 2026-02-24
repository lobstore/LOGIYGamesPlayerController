using KinematicCharacterController;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Wrapper for KinematicCharacterController's KinematicCharacterMotor.
    /// Implements the GenericControllerWrapper interface to allow seamless swapping
    /// with UnityControllerWrapper.
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(KinematicCharacterMotor))]
    public class KinematicControllerWrapper : ControllerWrapperBase, ICharacterController
    {
        [Header("Kinematic Controller Settings")]
        [SerializeField] private bool m_applyGravityWhenGrounded = false;
        [SerializeField] private float m_slopeLimit = 60f;
        
        private KinematicCharacterMotor m_kinematicMotor;
        private CapsuleCollider m_capsuleCollider;
        private CharacterGravityModule m_characterGravityModule;
        
        private bool m_collisionEnabled = true;
        private Vector3 m_cachedMoveDelta = Vector3.zero;
        private Quaternion m_cachedRotDelta = Quaternion.identity;
        
        // Cached values for properties
        private float m_cachedHeight;
        private float m_cachedRadius;
        private Vector3 m_cachedCenter;
        
        #region Public Properties
        
        public override bool IsGrounded => m_kinematicMotor.GroundingStatus.IsStableOnGround;
        
        public override Vector3 Velocity => m_kinematicMotor.Velocity;
        
        public override bool CollisionEnabled
        {
            get => m_collisionEnabled;
            set
            {
                m_collisionEnabled = value;
            }
        }
        
        public override float MaxStepHeight
        {
            get => m_kinematicMotor.MaxStepHeight;
            set => m_kinematicMotor.MaxStepHeight = value;
        }
        
        public override float Height
        {
            get => m_cachedHeight;
            set
            {
                m_cachedHeight = value;
                UpdateCapsuleDimensions();
            }
        }
        
        public override float SlopeLimit
        {
            get => m_slopeLimit;
            set => m_slopeLimit = Mathf.Max(0, value);
        }
        
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
            m_kinematicMotor = GetComponent<KinematicCharacterMotor>();
            m_capsuleCollider = GetComponent<CapsuleCollider>();
            m_characterGravityModule = GetComponent<CharacterGravityModule>();
            
            Debug.Assert(m_kinematicMotor != null, "Error (KinematicControllerWrapper): Could not find KinematicCharacterMotor component");
            Debug.Assert(m_capsuleCollider != null, "Error (KinematicControllerWrapper): Could not find CapsuleCollider component");
            
            // Assign this as the controller to the motor
            m_kinematicMotor.CharacterController = this;
            
            // Cache initial capsule values
            m_cachedRadius = m_capsuleCollider.radius;
            m_cachedHeight = m_capsuleCollider.height;
            m_cachedCenter = m_capsuleCollider.center;
        }
        
        #endregion
        
        #region Movement Methods
        
        public override void Move(Vector3 a_move)
        {
            if (m_collisionEnabled)
            {
                m_cachedMoveDelta = a_move * Time.deltaTime;
                m_kinematicMotor.SetPosition(m_kinematicMotor.TransientPosition + m_cachedMoveDelta);
            }
            else
            {
                m_kinematicMotor.Transform.Translate(a_move * Time.deltaTime, Space.World);
            }
        }  
        
        public override void Rotate(Quaternion a_targetRotation)
        {
            // Apply target rotation directly
            m_kinematicMotor.Transform.rotation = a_targetRotation;
            m_cachedRotDelta = a_targetRotation * Quaternion.Inverse(transform.rotation);
        }
        
        #endregion
        
        #region Transform Methods
        
        public override void SetPosition(Vector3 a_position)
        {
            m_kinematicMotor.SetPosition(a_position);
        }
        
        public override void SetRotation(Quaternion a_rotation)
        {
            m_kinematicMotor.SetRotation(a_rotation);
        }
        
        public override void SetPositionAndRotation(Vector3 a_position, Quaternion a_rotation)
        {
            m_kinematicMotor.SetPosition(a_position);
            m_kinematicMotor.SetRotation(a_rotation);
        }
        
        public override Vector3 GetCachedMoveDelta() => m_cachedMoveDelta;
        
        public override Quaternion GetCachedRotDelta() => m_cachedRotDelta;
        
        #endregion
        
        #region Jump Method
        
        public override void Jump(float force)
        {
            if (m_characterGravityModule != null)
            {
                m_kinematicMotor.ForceUnground(0.1f);
                Vector3 upDirection = m_kinematicMotor.CharacterUp;
                m_characterGravityModule.Velocity = upDirection * Mathf.Sqrt(force * -2f * Physics.gravity.y);
            }
        }
        
        #endregion
        
        #region Capsule Management
        
        private void UpdateCapsuleDimensions()
        {
            if (m_capsuleCollider == null) return;
            
            m_capsuleCollider.radius = m_cachedRadius;
            m_capsuleCollider.height = m_cachedHeight;
            m_kinematicMotor.SetCapsuleDimensions(m_cachedRadius, m_cachedHeight, m_cachedCenter.y);
        }
        
        private void UpdateCapsuleCenter()
        {
            if (m_capsuleCollider == null) return;
            
            m_capsuleCollider.center = m_cachedCenter;
            m_kinematicMotor.SetCapsuleDimensions(m_cachedRadius, m_cachedHeight, m_cachedCenter.y);
        }
        
        public override Collider GetCollider()
        {
            return m_capsuleCollider;
        }
        
        #endregion
        
        #region Initialization
        
        public override void Initialize()
        {
            // No additional setup needed
        }
        
        #endregion
        
        #region ICharacterController Implementation
        
        public void BeforeCharacterUpdate(float deltaTime)
        {
        }
        
        /// <summary>
        /// Called when the motor wants to know what its rotation should be.
        /// KinematicCharacterController handles rotation internally.
        /// </summary>
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            // Rotation is handled directly in Rotate() method
            // No additional processing needed here
        }
        
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            float currentVelocityMagnitude = currentVelocity.magnitude;
            Vector3 effectiveGroundNormal = m_kinematicMotor.GroundingStatus.GroundNormal;
            
            // Reorient velocity on slope
            currentVelocity = m_kinematicMotor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;
            
            if (m_characterGravityModule != null && !m_kinematicMotor.GroundingStatus.IsStableOnGround)
            {
                currentVelocity += m_characterGravityModule.Velocity * deltaTime;
            }
        }
        
        public void PostGroundingUpdate(float deltaTime)
        {
        }
        
        public void AfterCharacterUpdate(float deltaTime)
        {
        }
        
        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }
        
        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }
        
        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }
        
        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }
        
        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
        
        #endregion
        
        #region Editor Helpers
        
        private void OnValidate()
        {
            m_slopeLimit = Mathf.Max(0, m_slopeLimit);
        }
        
        #endregion
    }
}
