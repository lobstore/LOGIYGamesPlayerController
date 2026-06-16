using KinematicCharacterController;
using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(KinematicCharacterMotor))]
    public class KinematicControllerWrapper : MovementWrapperBase, ICharacterController
    {
        [Header("Kinematic Controller Settings")]
        
        private KinematicCharacterMotor m_kinematicMotor;
        private CapsuleCollider m_capsuleCollider;
        private SensorsModule m_SensorsModule;
        private CharacterGravityModule m_characterGravityModule;
        private CharacterModule character;
        
        #region Public Properties
        public override float MaxStepHeight
        {
            get => m_kinematicMotor.MaxStepHeight;
            set => m_kinematicMotor.MaxStepHeight = value;
        }
        Quaternion targetRotation;
        Vector3 targetVelocity;
        public int StableMovementSharpness;
        public float StableSlope;

        public override float Height
        {
            get => m_capsuleCollider.height;
            set
            {
                m_capsuleCollider.height = value;
                UpdateCapsuleDimensions();
            }
        }

        public override float SlopeLimit
        {
            get => m_kinematicMotor.MaxStableSlopeAngle;
            set => m_kinematicMotor.MaxStableSlopeAngle = Mathf.Max(0, value);
        }

        public override Vector3 Center
        {
            get { return m_capsuleCollider.center; }
            set { m_capsuleCollider.center = value; UpdateCapsuleDimensions(); }
        }

        public override float Radius
        {
            get { return m_capsuleCollider.radius; }
            set { m_capsuleCollider.radius = value; UpdateCapsuleDimensions(); }
        }

        public override bool UseGravity { get => m_characterGravityModule.UseGravity; set => m_characterGravityModule.UseGravity = value; }

        public override Vector3 Velocity => m_kinematicMotor.Velocity;


        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            m_kinematicMotor = GetComponent<KinematicCharacterMotor>();
            m_capsuleCollider = GetComponent<CapsuleCollider>();
            m_characterGravityModule = GetComponent<CharacterGravityModule>();
            m_kinematicMotor.MaxStableSlopeAngle = StableSlope;
            Debug.Assert(m_kinematicMotor != null, "Error (KinematicControllerWrapper): Could not find KinematicCharacterMotor component");
            Debug.Assert(m_capsuleCollider != null, "Error (KinematicControllerWrapper): Could not find CapsuleCollider component");
            m_kinematicMotor.CharacterController = this;
            
        }
        
        #endregion
        
        #region Movement Methods
        
        public override void Move(Vector3 a_move)
        {
            targetVelocity = a_move;
        }  
        
        public override void SetRotation(Quaternion a_targetRotation)
        {
            targetRotation = a_targetRotation;
        }
        
        #endregion
        
        #region Transform Methods
        
        public override void SetPosition(Vector3 a_position)
        {
            m_kinematicMotor.SetPosition(a_position);
        }
        
        #endregion
        
        #region Jump Method
        
        public override void AddForce(Vector3 force)
        {
            if (m_characterGravityModule != null)
            {
                m_kinematicMotor.ForceUnground(0.1f);
                character.VelocityData.Gravity =  force;
            }
        }
        
        #endregion
        
        #region Capsule Management
        
        private void UpdateCapsuleDimensions()
        {
            if (m_capsuleCollider == null) return;
            
            m_capsuleCollider.radius = Radius;
            m_capsuleCollider.height = Height;
            m_kinematicMotor.SetCapsuleDimensions(Radius, Height, Center.y);
        }
        
        public void BeforeCharacterUpdate(float deltaTime)
        {
        }
        
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            currentRotation = targetRotation;
        }
        
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            float currentVelocityMagnitude = currentVelocity.magnitude;
            Vector3 effectiveGroundNormal = m_kinematicMotor.GroundingStatus.GroundNormal;
            currentVelocity = m_kinematicMotor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
            if (m_characterGravityModule != null && !m_kinematicMotor.GroundingStatus.IsStableOnGround)
            {
                currentVelocity += character.VelocityData.Gravity;
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
            targetVelocity = Vector3.zero;
        }

        #endregion

    }
}