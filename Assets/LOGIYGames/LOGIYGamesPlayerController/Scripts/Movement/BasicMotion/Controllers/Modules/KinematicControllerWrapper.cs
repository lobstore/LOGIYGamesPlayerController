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

        public override bool UseGravity { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override Vector3 Velocity => throw new System.NotImplementedException();


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
                m_cachedMoveDelta = a_move;
            }
            else
            {
                m_kinematicMotor.Transform.Translate(a_move * Time.deltaTime, Space.World);
            }
        }  
        
        public override void SetRotation(Quaternion a_targetRotation)
        {
            m_cachedRotDelta = a_targetRotation;
        }
        
        #endregion
        
        #region Transform Methods
        
        public override void SetPosition(Vector3 a_position)
        {
            m_kinematicMotor.SetPosition(a_position);
        }
        
        #endregion
        
        #region Jump Method
        
        public override void Jump(Vector3 force)
        {
            if (m_characterGravityModule != null)
            {
                m_kinematicMotor.ForceUnground(0.1f);
                m_characterGravityModule.Velocity =  force;
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
        
        public void BeforeCharacterUpdate(float deltaTime)
        {
        }
        
        /// <summary>
        /// Called when the motor wants to know what its rotation should be.
        /// KinematicCharacterController handles rotation internally.
        /// </summary>
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            currentRotation = m_cachedRotDelta;
        }
        
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            float currentVelocityMagnitude = currentVelocity.magnitude;
            Vector3 effectiveGroundNormal = m_kinematicMotor.GroundingStatus.GroundNormal;

            // Reorient velocity on slope
            currentVelocity = m_kinematicMotor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;
            currentVelocity = Vector3.Lerp(currentVelocity, m_cachedMoveDelta, deltaTime*5);
            if (m_characterGravityModule != null && !m_kinematicMotor.GroundingStatus.IsStableOnGround)
            {
                currentVelocity += m_characterGravityModule.Velocity;
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

        public override void ResetVelocity()
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}