using LOGIYGames.CharacterCore;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace LOGIYGames
{
    public class NavMeshControllerWrapper : ControllerWrapperBase
    {
        SensorsModule sensorModule;
        NavMeshAgent agent;
        CharacterGravityModule characterGravityModule;
        CharacterController characterController;
        Character character;
        private Vector3 targetVelocity;

        public override bool IsGrounded => sensorModule.IsGrounded;

        public override Vector3 Velocity => agent.velocity;

        public override bool CollisionEnabled { get; set; }
        public override float MaxStepHeight { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override float Height { get => agent.height; set => agent.height = value; }
        public override float SlopeLimit { get => sensorModule.MaxStableSlopeAngle; set => sensorModule.MaxStableSlopeAngle = value; }
        public override Vector3 Center { get => characterController.center; set => characterController.center = value; }
        public override float Radius { get => agent.radius; set => agent.radius = value; }

        public override bool ApplyGravityWhenGrounded { get; }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            character = GetComponent<Character>();
            sensorModule = GetComponent<SensorsModule>();
            agent = GetComponent<NavMeshAgent>();
            characterGravityModule = GetComponent<CharacterGravityModule>();
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (!sensorModule.IsGrounded||characterGravityModule.Velocity.y>0)
            {
                agent.enabled = false;
            }
            else
            {
                agent.enabled = true;
            }
        }
        public override Vector3 GetCachedMoveDelta()
        {
            throw new System.NotImplementedException();
        }

        public override Quaternion GetCachedRotDelta()
        {
            throw new System.NotImplementedException();
        }

        public override Collider GetCollider()
        {
            return characterController;
        }

        public override void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public override void Jump(float force)
        {
            agent.enabled = false;
            if (characterGravityModule != null)
            {
                characterGravityModule.Velocity = transform.up * Mathf.Sqrt(force * -2f * Physics.gravity.y);
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
                Vector3 totalVelocity = a_move + characterGravityModule.Velocity;
                if (sensorModule.IsValidSlope())
                {
                    if (sensorModule.IsGrounded && characterGravityModule.Velocity.y < 0 && character.Input.MovementInput.magnitude > 0)
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
            //characterController.Move(characterGravityModule.Velocity * Time.deltaTime);
        }
        private void ProjectVelocity(Vector3 totalVelocity)
        {
            targetVelocity = Vector3.ProjectOnPlane(totalVelocity, sensorModule.BelowHit.normal) + Vector3.ProjectOnPlane(-transform.up, sensorModule.BelowHit.normal);
        }
        public override void Rotate(Quaternion a_targetRotation)
        {
            transform.rotation = a_targetRotation;
        }

        public override void SetPosition(Vector3 a_position)
        {
            agent.Warp(a_position);
        }

        public override void SetPositionAndRotation(Vector3 a_position, Quaternion a_rotation)
        {
            agent.Warp(a_position);
            transform.rotation = a_rotation;
        }

        public override void SetRotation(Quaternion a_rotation)
        {
            transform.rotation = a_rotation;
        }


    }
}
