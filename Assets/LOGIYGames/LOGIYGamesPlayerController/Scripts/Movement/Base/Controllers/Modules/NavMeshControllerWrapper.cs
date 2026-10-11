using LOGIYGames.AI;
using LOGIYGames.CharacterCore;
using System.Collections.Generic;
using UniGLTF;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace LOGIYGames
{
    public class NavMeshControllerWrapper : MovementWrapperBase
    {
        SensorsModule m_sensors;
        NavMeshAgent agent;
        [SerializeField] CharacterController m_characterController;
        Actor character;
        AIBrain AIBrain;
        private Vector3 linearVelocity;
        private LayerMask excludeLayers;
        private LayerMask includeLayers;
        [SerializeField] private float aerialDamping;
        [SerializeField] private float groundDamping;
        [SerializeField] private float damping;
        [SerializeField] private bool useGravity;
        [SerializeField] bool freeze;
        GroundedReport lastGroundedReport;
        

        public override GroundedReport LastGroundedReport => lastGroundedReport;
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

        public override Collider Collider => m_characterController;

        void Awake()
        {
            character = GetComponent<Actor>();
            m_sensors = GetComponent<SensorsModule>();
            agent = GetComponent<NavMeshAgent>();
            AIBrain = GetComponent<AIBrain>();
        }

        private void Update()
        {
            if (freeze) return;
            if (m_sensors.IsGrounded &&  linearVelocity.y<=0)
            {
                agent.enabled = true;
                linearVelocity.y = 0f;
            }
            else
            {

                AddAcceleration(Physics.gravity);

            }


           // agent.baseOffset = Mathf.MoveTowards(agent.baseOffset, linearVelocity.y, Time.deltaTime);



            //agent.Move(linearVelocity * Time.deltaTime);

            //if (agent.isOnNavMesh)
            //{
            //    agent.Move(linearVelocity * Time.deltaTime);
            //}
            //else
            //{
            //    damping = m_sensors.IsGrounded ? groundDamping : aerialDamping;
            //    linearVelocity = Vector3.MoveTowards(linearVelocity, Vector3.zero, damping * Time.deltaTime);
            //    m_characterController.Move(linearVelocity * Time.deltaTime);
            //}
        }
        public override void AddImpulse(Vector3 impulseForce)
        {
            linearVelocity += impulseForce;
        }
        public override void ForceMove(Vector3 velocity)
        {
            //linearVelocity = velocity;
        }
        public override void ChangeHorizontalVelocity(Vector3 a_move)
        {
           // linearVelocity = a_move;
        }
        public override void AddAcceleration(Vector3 accelerationForce)
        {
            linearVelocity += accelerationForce * Time.deltaTime;
        }
        public override void SetRotation(Quaternion a_targetRotation)
        {
            transform.rotation = a_targetRotation;
        }

        public override void SetPosition(Vector3 a_position)
        {
            agent.Warp(a_position);
        }

        public override void ResetVelocity()
        {
            linearVelocity = Vector3.zero;
        }

        public override void ResetGravity()
        {
            linearVelocity.y = 0;
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

        public override void ChangeVerticalVelocity(float velocityY)
        {
            
        }
    }
}
