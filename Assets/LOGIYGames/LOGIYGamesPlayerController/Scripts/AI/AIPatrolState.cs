using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Patrol state - AI moves between patrol points
    /// Transitions back to Idle when reaching a point
    /// </summary>
    public class AIPatrolState : AIBaseState
    {
        private Transform _currentPatrolPoint;
        private float _arrivalThreshold;

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

            // Check if target detected - transition to Chase
            if (Brain.Target != null && Brain.HasLineOfSight())
            {
                return;
            }

            // Check if patrol points exist
            if (Brain.PatrolPoints == null || Brain.PatrolPoints.Length == 0)
            {
                return;
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
                MoveAlongNavMeshPath(_currentPatrolPoint.position, _arrivalThreshold);
            }
            else
            {
                AIInput.SetMovementInput(Vector2.zero);
            }
        }

        /// <summary>
        /// Selects a random patrol point
        /// </summary>
        private void SelectRandomPatrolPoint()
        {
            if (Brain.PatrolPoints == null || Brain.PatrolPoints.Length == 0)
            {
                _currentPatrolPoint = null;
                return;
            }

            _currentPatrolPoint = Brain.PatrolPoints[Random.Range(0, Brain.PatrolPoints.Length)];
        }

        /// <summary>
        /// Checks if AI has reached the patrol point
        /// </summary>
        private bool IsAtPatrolPoint()
        {
            return _currentPatrolPoint == null ||
                   Vector3.Distance(AITransform.position, _currentPatrolPoint.position) <= _arrivalThreshold;
        }

        /// <summary>
        /// Checks if AI has reached the patrol point (for transition predicate)
        /// </summary>
        public bool HasReachedPatrolPoint()
        {
            return _currentPatrolPoint == null || IsAtPatrolPoint();
        }

        /// <summary>
        /// Sets the arrival threshold distance
        /// </summary>
        public void SetArrivalThreshold(float threshold)
        {
            _arrivalThreshold = Mathf.Max(0.1f, threshold);
        }
    }
}
