using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Patrol state - AI moves between patrol points
    /// </summary>
    public class AIPatrolState : AIBaseState
    {
        private Transform _currentPatrolPoint;
        private readonly float _arrivalThreshold;

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
            Brain.TargetingController.SetTarget(_currentPatrolPoint);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (Brain.PatrolPoints == null || Brain.PatrolPoints.Length == 0)
            {
                Stop();
                return;
            }

            if (_currentPatrolPoint != null && !IsAtPatrolPoint())
            {
                MoveToPosition(_currentPatrolPoint.position);
            }
            else
            {
                Stop();
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
                   Brain.GetDistanceToTarget() <= _arrivalThreshold;
        }

        /// <summary>
        /// Checks if AI has reached the patrol point (for transition predicate)
        /// </summary>
        public bool HasReachedPatrolPoint()
        {
            return _currentPatrolPoint == null || IsAtPatrolPoint();
        }
    }
}
