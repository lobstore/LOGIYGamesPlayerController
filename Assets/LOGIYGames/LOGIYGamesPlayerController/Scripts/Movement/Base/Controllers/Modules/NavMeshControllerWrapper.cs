using LOGIYGames.CharacterCore;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace LOGIYGames
{
    public class NavMeshControllerWrapper : MovementWrapperBase
    {
        SensorsModule sensorModule;
        NavMeshAgent agent;
        CharacterGravityModule characterGravityModule;
        CharacterController characterController;
        Character character;
        private Vector3 targetVelocity;

        public override float MaxStepHeight { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override float Height { get => agent.height; set => agent.height = value; }
        public override float SlopeLimit { get => sensorModule.MaxStableSlopeAngle; set => sensorModule.MaxStableSlopeAngle = value; }
        public override Vector3 Center { get => characterController.center; set => characterController.center = value; }
        public override float Radius { get => agent.radius; set => agent.radius = value; }
        public override bool UseGravity { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override Vector3 Velocity => throw new System.NotImplementedException();

        void Awake()
        {
            character = GetComponent<Character>();
            sensorModule = GetComponent<SensorsModule>();
            agent = GetComponent<NavMeshAgent>();
            characterGravityModule = GetComponent<CharacterGravityModule>();
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (sensorModule.IsGrounded&& characterGravityModule.CurrentGravity.y<0)
            {
                agent.enabled = true;
            }
        }
        public override void AddForce(Vector3 force)
        {
            agent.enabled = false;
            if (characterGravityModule != null)
            {
                characterGravityModule.CurrentGravity = force ;
            }
        }

        public override void Move(Vector3 a_move)
        {
            if (agent!=null && agent.isOnNavMesh)
            {

            agent.Move(a_move*Time.deltaTime);
            }
            else
            {
                Vector3 totalVelocity = a_move + characterGravityModule.CurrentGravity;
                if (sensorModule.IsValidSlope())
                {
                    if (sensorModule.IsGrounded && characterGravityModule.CurrentGravity.y < 0 && character.Input.MovementInput.magnitude > 0)
                    {
                        ProjectVelocity(totalVelocity);

                    }
                    else
                    {
                        targetVelocity = totalVelocity;
                    }
                }
                else
                {
                    ProjectVelocity(totalVelocity);
                }
                characterController.Move(targetVelocity * Time.deltaTime);
            }
        }
        private void ProjectVelocity(Vector3 totalVelocity)
        {
            targetVelocity = Vector3.ProjectOnPlane(totalVelocity, sensorModule.BelowHit.normal) + Vector3.ProjectOnPlane(-transform.up, sensorModule.BelowHit.normal);
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
            agent.ResetPath();
        }
    }
}
