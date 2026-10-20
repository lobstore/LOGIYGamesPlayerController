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

        [SerializeField] CapsuleCollider characterCapsuleCollider;

        private Rigidbody m_rigidbody;
        [SerializeField] Actor actor;
        [SerializeField] float maxStepHeight;
        public override Collider Collider => characterCapsuleCollider;

        private SensorsModule m_sensors;
        [SerializeField] private float baseSpeed = 3.5f;

        #region Public Properties

        public override float MaxStepHeight { get => maxStepHeight; }
        public override float Height
        {
            get => characterCapsuleCollider.height;
            set
            {
                characterCapsuleCollider.height = value;
            }
        }

        public override float SlopeLimit { get; set; }

        public override Vector3 Center
        {
            get => characterCapsuleCollider.center;
            set
            {
                characterCapsuleCollider.center = value;
            }
        }

        public override float Radius
        {
            get => characterCapsuleCollider.radius;
            set
            {
                characterCapsuleCollider.radius = value;
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
            m_sensors = GetComponent<SensorsModule>();

            // Configure Rigidbody for actor controller
            m_rigidbody.freezeRotation = true;
            m_rigidbody.interpolation = RigidbodyInterpolation.None;
            m_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        #endregion


        public override void ChangeHorizontalVelocity(Vector3 a_move)
        {
            m_rigidbody.linearVelocity = new Vector3(a_move.x, m_rigidbody.linearVelocity.y, a_move.z);
            if (m_sensors.IsGrounded && m_rigidbody.linearVelocity.y<=0)
            {
                m_rigidbody.linearVelocity = Vector3.ProjectOnPlane(m_rigidbody.linearVelocity, m_sensors.GroundHit.normal);
                
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
        }

        public override void EnableMovement()
        {

        }

        public override void ChangeVerticalVelocity(float velocityY)
        {
            m_rigidbody.linearVelocity = new(m_rigidbody.linearVelocity.x, velocityY, m_rigidbody.linearVelocity.z);
        }
    }
}
