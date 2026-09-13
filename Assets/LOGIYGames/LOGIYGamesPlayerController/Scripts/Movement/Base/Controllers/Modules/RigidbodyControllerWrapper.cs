using GenshinImpactMovementSystem;
using LOGIYGames.CharacterCore;
using System;
using UnityEngine;

namespace LOGIYGames
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyControllerWrapper : MovementWrapperBase
    {
        [Header("Rigidbody Controller Settings")]

        [SerializeField] CharacterCapsuleCollider characterCapsuleCollider;

        private Rigidbody m_rigidbody;
        [SerializeField] Actor actor;
        [SerializeField] float maxStepHeight;
        public override Collider Collider => characterCapsuleCollider.Collider;

        private SensorsModule m_sensors;
        [SerializeField] private float baseSpeed = 3.5f;

        #region Public Properties

        public override float MaxStepHeight { get => maxStepHeight; }
        public override float Height
        {
            get => characterCapsuleCollider.Collider.height;
            set
            {
                characterCapsuleCollider.Collider.height = value;
            }
        }

        public override float SlopeLimit { get; set; }

        public override Vector3 Center
        {
            get => characterCapsuleCollider.Collider.center;
            set
            {
                characterCapsuleCollider.Collider.center = value;
            }
        }

        public override float Radius
        {
            get => characterCapsuleCollider.Collider.radius;
            set
            {
                characterCapsuleCollider.Collider.radius = value;
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
            characterCapsuleCollider = GetComponent<CharacterCapsuleCollider>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_sensors = GetComponent<SensorsModule>();

            // Configure Rigidbody for character controller
            m_rigidbody.freezeRotation = true;
            m_rigidbody.interpolation = RigidbodyInterpolation.None;
            m_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void Update()
        {
            m_rigidbody.linearDamping = actor.RuntimeMovement.Deceleration;
            
        }
        private void FixedUpdate()
        {
            //Float();
        }
        #endregion


        public override void ChangeVelocity(Vector3 a_move)
        {
            Vector3 targetVelocity;
            targetVelocity = a_move;
            if (m_sensors.IsOnSlope && UseProjectionOnPlane)
            {
                targetVelocity = Vector3.ProjectOnPlane(
                    targetVelocity,
                    m_sensors.BelowHit.normal
                );
            }
            if (m_sensors.IsGrounded)
            {
                if (m_rigidbody.linearVelocity.magnitude <= actor.RuntimeMovement.CurrentSpeed * baseSpeed)
                {
                    m_rigidbody.AddForce(targetVelocity * actor.RuntimeMovement.Acceleration, ForceMode.Acceleration);
                }
                else
                {
                   // m_rigidbody.linearVelocity = targetVelocity * actor.RuntimeMovement.CurrentSpeed * baseSpeed;
                }
            }
            else
            {
                m_rigidbody.AddForce(targetVelocity * actor.RuntimeMovement.Acceleration, ForceMode.Acceleration);
            }
  
        }
        private void Float()
        {
            if (m_sensors.IsGrounded)
            {

                float distanceToFloatingPoint = characterCapsuleCollider.Collider.center.y * transform.localScale.y - m_sensors.GroundHit.distance;

                if (distanceToFloatingPoint <= 0f)
                {
                    return;
                }

                float amountToLift = distanceToFloatingPoint * characterCapsuleCollider.StepData.StepReachForce - m_rigidbody.linearVelocity.y;

                Vector3 liftForce = new Vector3(0f, amountToLift, 0f);

                m_rigidbody.AddForce(liftForce * 10, ForceMode.Acceleration);
            }
        }
        public override void ForceMove(Vector3 a_move)
        {
            m_rigidbody.linearVelocity = a_move;
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

        public override void SetPosition(Vector3 a_position)
        {
            m_rigidbody.position = a_position;
        }



        public override void AddImpulse(Vector3 force)
        {
            m_rigidbody.AddForce(force * m_rigidbody.mass, ForceMode.Impulse);
        }

        public override void ResetGravity()
        {
            m_rigidbody.linearVelocity = new Vector3(m_rigidbody.linearVelocity.x, 0, m_rigidbody.linearVelocity.z);
        }

        public override void DisableMovement()
        {
            throw new NotImplementedException();
        }

        public override void EnableMovement()
        {
            throw new NotImplementedException();
        }
    }
}
