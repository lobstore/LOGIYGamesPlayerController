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
            Character = brain.GetComponent<Character>();
            Sensors = brain.GetComponent<SensorsModule>();
        }

        public virtual void Enter()
        {
            StateTime = 0f;
            AIInput.ClearAllInputs();

            // Set AI-specific movement and rotation strategies (world-space, no camera influence)
            // Speed is controlled by MovementStateDriver via MovementStateDataSO
            if (Character != null)
            {
                Character.CurrentMovementStrategy = new AIWorldMovement(Character);
                Character.CurrentRotationStrategy = new AIMovementRotation(Character);
            }
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
        protected void SetMovementDirection(Vector3 direction)
        {
            Vector3 flattenedDirection = new Vector3(direction.x, 0, direction.z).normalized;
            AIInput.SetMovementInput(flattenedDirection);
        }

        /// <summary>
        /// Calculates movement direction from NavMesh path and applies it to input
        /// </summary>
        /// <param name="targetPosition">Target position to move towards</param>
        /// <param name="arrivalThreshold">Distance threshold to consider arrival</param>
        /// <returns>True if path is calculated and direction is set, false if arrived or no path</returns>
        protected bool MoveAlongNavMeshPath(Vector3 targetPosition, float arrivalThreshold = 0.5f)
        {
            if (Brain.NavMeshAgent == null || !Brain.NavMeshAgent.isOnNavMesh)
            {
                MoveTowardsPosition(targetPosition);
                return true;
            }

            // Update path with throttling
            Brain.UpdatePath(targetPosition, IsGrounded());

            // Check if we've reached the destination
            float distanceToTarget = Vector3.Distance(AITransform.position, targetPosition);
            if (distanceToTarget <= arrivalThreshold)
            {
                AIInput.SetMovementInput(Vector2.zero);
                return false;
            }

            // No movement input while airborne
            if (!IsGrounded())
            {
                AIInput.SetMovementInput(Vector2.zero);
                return true;
            }

            // Recalculate path if stuck
            if (Brain.IsStuck())
            {
                Brain.RecalculatePath();
            }

            // Get direction from NavMesh path
            Vector3 direction = Brain.GetPathDirection();

            if (direction.magnitude > 0.01f)
            {
                SetMovementDirection(direction);
                return true;
            }

            // Fallback to direct movement
            MoveTowardsPosition(targetPosition);
            return true;
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
        /// Checks if target is within range
        /// </summary>
        protected bool IsTargetInRange(Transform target, float range)
        {
            return target != null && Vector3.Distance(AITransform.position, target.position) <= range;
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

            return true;
        }

        /// <summary>
        /// Gets distance to target
        /// </summary>
        protected float GetDistanceToTarget(Transform target)
        {
            return target == null ? float.MaxValue : Vector3.Distance(AITransform.position, target.position);
        }

        /// <summary>
        /// Gets direction to target (flattened)
        /// </summary>
        protected Vector3 GetDirectionToTarget(Transform target)
        {
            if (target == null) return AITransform.forward;

            Vector3 direction = target.position - AITransform.position;
            direction.y = 0;
            return direction.normalized;
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
