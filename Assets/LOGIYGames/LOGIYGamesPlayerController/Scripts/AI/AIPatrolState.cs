using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Patrol state - AI moves to a random patrol point
    /// Transitions back to Idle when reaching the point
    /// </summary>
    public class AIPatrolState : AIBaseState
    {
        /// <summary>
        /// Current target patrol point
        /// </summary>
        private Transform _currentPatrolPoint;

        /// <summary>
        /// Distance threshold to consider patrol point reached
        /// </summary>
        private float _arrivalThreshold = 0.5f;

        public AIPatrolState(AIBrain brain, float arrivalThreshold = 0.5f) : base(brain)
        {
            _arrivalThreshold = arrivalThreshold;
            DetectionRange = brain.DetectionRange;
            AttackRange = brain.AttackRange;
        }

        public override void Enter()
        {
            base.Enter();
            SelectRandomPatrolPoint();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Check if target is detected - transition to Chase
            if (Brain.Target != null && CanDetectTarget())
            {
                return;
            }

            // Check if patrol points exist
            if (Brain.PatrolPoints == null || Brain.PatrolPoints.Length == 0)
            {
                // No patrol points, transition to Idle
                return;
            }

            // Check if reached patrol point - transition to Idle
            if (_currentPatrolPoint == null || IsAtPatrolPoint())
            {
                // Signal that patrol is complete (transition handled by AIBrain predicates)
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (Brain.PatrolPoints == null || Brain.PatrolPoints.Length == 0)
            {
                AIInput.SetMovementInput(Vector2.zero);
                return;
            }

            if (_currentPatrolPoint != null && !IsAtPatrolPoint())
            {
                // Move towards patrol point using NavMesh pathfinding
                MoveAlongNavMeshPath(_currentPatrolPoint.position, _arrivalThreshold);
            }
            else
            {
                AIInput.SetMovementInput(Vector2.zero);
            }
        }

        /// <summary>
        /// Selects a random patrol point from available points
        /// </summary>
        private void SelectRandomPatrolPoint()
        {
            if (Brain.PatrolPoints == null || Brain.PatrolPoints.Length == 0)
            {
                _currentPatrolPoint = null;
                return;
            }

            int randomIndex = Random.Range(0, Brain.PatrolPoints.Length);
            _currentPatrolPoint = Brain.PatrolPoints[randomIndex];
        }

        /// <summary>
        /// Checks if AI has reached the current patrol point
        /// </summary>
        private bool IsAtPatrolPoint()
        {
            if (_currentPatrolPoint == null) return true;

            float distance = Vector3.Distance(AITransform.position, _currentPatrolPoint.position);
            return distance <= _arrivalThreshold;
        }

        /// <summary>
        /// Gets the current target patrol point
        /// </summary>
        public Transform GetCurrentPatrolPoint()
        {
            return _currentPatrolPoint;
        }

        /// <summary>
        /// Sets the arrival threshold distance
        /// </summary>
        public void SetArrivalThreshold(float threshold)
        {
            _arrivalThreshold = Mathf.Max(0.1f, threshold);
        }

        /// <summary>
        /// Checks if target can be detected (in range and line of sight)
        /// </summary>
        private bool CanDetectTarget()
        {
            if (Brain.Target == null) return false;

            float distance = GetDistanceToTarget(Brain.Target);

            if (distance <= DetectionRange)
            {
                return HasLineOfSight(Brain.Target, DetectionRange);
            }

            return false;
        }

        /// <summary>
        /// Checks if AI has reached the current patrol point
        /// </summary>
        public bool HasReachedPatrolPoint()
        {
            return _currentPatrolPoint == null || IsAtPatrolPoint();
        }
    }
}
