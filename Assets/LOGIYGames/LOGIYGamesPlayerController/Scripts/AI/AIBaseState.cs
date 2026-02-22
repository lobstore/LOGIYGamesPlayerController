using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// Abstract base class for all AI behavior states
    /// </summary>
    public abstract class AIBaseState : IState
    {
        protected AIBrain Brain { get; private set; }
        protected Character Character { get; private set; }
        protected Transform CharacterTransform { get; private set; }

        /// <summary>
        /// Detection range for this AI state
        /// </summary>
        protected float DetectionRange { get; set; } = 10f;

        /// <summary>
        /// Attack range for this AI
        /// </summary>
        protected float AttackRange { get; set; } = 2f;

        /// <summary>
        /// Time spent in current state
        /// </summary>
        protected float StateTime { get; private set; }

        protected AIBaseState(AIBrain brain)
        {
            Brain = brain;
            CharacterTransform = brain.transform;
            Character = brain.GetComponent<Character>();
        }

        public virtual void Enter()
        {
            StateTime = 0f;

            // Set AI-specific movement and rotation strategies (world-space, no camera influence)
            if (Character != null)
            {
                Character.CurrentMovementStrategy = new AIWorldMovement(Character);
                Character.CurrentRotationStrategy = new AIMovementRotation(Character);
            }
        }

        public virtual void Exit()
        {
        }

        public virtual void LogicUpdate()
        {
            StateTime += Time.deltaTime;
        }

        public virtual void PhysicsUpdate()
        {
        }

        public virtual void LateUpdate()
        {
        }

        /// <summary>
        /// Sets destination for NavMeshAgent
        /// </summary>
        protected void MoveToPosition(Vector3 position)
        {
            Brain.SetDestination(position);
        }

        /// <summary>
        /// Clears destination (stops movement)
        /// </summary>
        protected void Stop()
        {
            Brain.ClearDestination();
        }

        /// <summary>
        /// Checks if target is within range
        /// </summary>
        protected bool IsTargetInRange(Transform target, float range)
        {
            return target != null && Vector3.Distance(CharacterTransform.position, target.position) <= range;
        }

        /// <summary>
        /// Checks if target is within attack range
        /// </summary>
        protected bool IsTargetInAttackRange(Transform target)
        {
            return IsTargetInRange(target, AttackRange);
        }

        /// <summary>
        /// Checks if AI can see the target (line of sight)
        /// </summary>
        protected bool HasLineOfSight(Transform target, float maxDistance = 50f)
        {
            if (target == null) return false;

            Vector3 direction = target.position - CharacterTransform.position;
            float distance = direction.magnitude;

            if (distance > maxDistance) return false;

            direction.Normalize();

            if (Physics.Raycast(CharacterTransform.position + Vector3.up * 0.5f, direction, out RaycastHit hit, distance))
            {
                return hit.transform == target || hit.transform.IsChildOf(target);
            }

            return true;
        }

        /// <summary>
        /// Gets distance to target
        /// </summary>
        protected float GetDistanceToTarget(Transform target)
        {
            return target == null ? float.MaxValue : Vector3.Distance(CharacterTransform.position, target.position);
        }

        /// <summary>
        /// Gets direction to target (flattened)
        /// </summary>
        protected Vector3 GetDirectionToTarget(Transform target)
        {
            if (target == null) return CharacterTransform.forward;

            Vector3 direction = target.position - CharacterTransform.position;
            direction.y = 0;
            return direction.normalized;
        }
    }
}
