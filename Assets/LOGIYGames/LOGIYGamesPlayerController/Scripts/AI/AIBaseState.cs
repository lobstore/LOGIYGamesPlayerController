using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// Abstract base class for all AI behavior states
    /// Provides common functionality and references to AI components
    /// </summary>
    public abstract class AIBaseState : IState
    {
        protected AIBrain Brain { get; private set; }
        protected AIInputReader AIInput { get; private set; }
        protected SensorsModule Sensors { get; private set; }
        protected Character Character { get; private set; }
        protected Transform AITransform { get; private set; }

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
            AIInput = brain.AIInput;
            AITransform = brain.transform;
        }

        public virtual void Enter()
        {
            StateTime = 0f;
            AIInput.ClearAllInputs();
        }

        public virtual void Exit()
        {
            AIInput.ClearAllInputs();
        }

        public virtual void LogicUpdate()
        {
            StateTime += Time.deltaTime;
        }

        public virtual void PhysicsUpdate()
        {
            // Movement is handled by MovementStateDriver
        }

        public virtual void LateUpdate()
        {
        }

        /// <summary>
        /// Sets movement input direction for the AI
        /// </summary>
        /// <param name="direction">Movement direction in world space</param>
        protected void SetMovementDirection(Vector3 direction)
        {
            // Convert world direction to local input (relative to camera or character)
            Vector3 flattenedDirection = new Vector3(direction.x, 0, direction.z).normalized;
            AIInput.SetMovementInput(flattenedDirection);
        }

        /// <summary>
        /// Sets movement input towards a target position
        /// </summary>
        protected void MoveTowardsPosition(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - AITransform.position;
            direction.y = 0;

            if (direction.magnitude > 0.1f)
            {
                SetMovementDirection(direction.normalized);
            }
            else
            {
                AIInput.SetMovementInput(Vector2.zero);
            }
        }

        /// <summary>
        /// Checks if target is within detection range
        /// </summary>
        protected bool IsTargetInRange(Transform target, float range)
        {
            if (target == null) return false;

            float distance = Vector3.Distance(AITransform.position, target.position);
            return distance <= range;
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

            Vector3 direction = target.position - AITransform.position;
            float distance = direction.magnitude;

            if (distance > maxDistance) return false;

            direction.Normalize();

            if (Physics.Raycast(AITransform.position + Vector3.up * 0.5f, direction, out RaycastHit hit, distance))
            {
                return hit.transform == target || hit.transform.IsChildOf(target);
            }

            return false;
        }

        /// <summary>
        /// Gets the direction to target
        /// </summary>
        protected Vector3 GetDirectionToTarget(Transform target)
        {
            if (target == null) return AITransform.forward;

            Vector3 direction = target.position - AITransform.position;
            direction.y = 0;
            return direction.normalized;
        }

        /// <summary>
        /// Gets distance to target
        /// </summary>
        protected float GetDistanceToTarget(Transform target)
        {
            if (target == null) return float.MaxValue;
            return Vector3.Distance(AITransform.position, target.position);
        }

        /// <summary>
        /// Checks if AI is grounded
        /// </summary>
        protected bool IsGrounded()
        {
            return Sensors.IsGrounded;
        }
    }
}
