using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Patrol state - AI moves between predefined patrol points
    /// Transitions to Chase when target is detected
    /// </summary>
    public class AIPatrolState : AIBaseState
    {
        /// <summary>
        /// Current patrol point index
        /// </summary>
        private int _currentPatrolIndex;

        /// <summary>
        /// Wait time at each patrol point
        /// </summary>
        private float _waitTime = 1f;

        /// <summary>
        /// Current wait timer at patrol point
        /// </summary>
        private float _waitTimer;

        /// <summary>
        /// Whether AI is currently waiting at a patrol point
        /// </summary>
        private bool _isWaiting;

        /// <summary>
        /// Patrol movement speed multiplier
        /// </summary>
        private float _patrolSpeed = 0.6f;

        public AIPatrolState(AIBrain brain, float waitTime = 1f, float patrolSpeed = 0.6f) : base(brain)
        {
            _waitTime = waitTime;
            _patrolSpeed = patrolSpeed;
            DetectionRange = brain.DetectionRange;
            AttackRange = brain.AttackRange;
        }

        public override void Enter()
        {
            base.Enter();
            _waitTimer = 0f;
            _isWaiting = false;
            
            // Find closest patrol point if not set
            if (Brain.PatrolPoints != null && Brain.PatrolPoints.Length > 0)
            {
                _currentPatrolIndex = FindClosestPatrolPoint();
            }
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

            // Handle patrol behavior
            if (Brain.PatrolPoints == null || Brain.PatrolPoints.Length == 0)
            {
                // No patrol points, stay idle
                AIInput.SetMovementInput(Vector2.zero);
                return;
            }

            if (_isWaiting)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= _waitTime)
                {
                    _isWaiting = false;
                    _waitTimer = 0f;
                    AdvanceToNextPatrolPoint();
                }
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

            if (!_isWaiting)
            {
                Transform currentPoint = Brain.PatrolPoints[_currentPatrolIndex];
                
                if (currentPoint != null)
                {
                    float distanceToPoint = Vector3.Distance(AITransform.position, currentPoint.position);
                    
                    if (distanceToPoint <= 0.5f)
                    {
                        // Reached patrol point, start waiting
                        _isWaiting = true;
                        AIInput.SetMovementInput(Vector2.zero);
                    }
                    else
                    {
                        // Move towards patrol point
                        MoveTowardsPosition(currentPoint.position);
                    }
                }
            }
            else
            {
                AIInput.SetMovementInput(Vector2.zero);
            }
        }

        /// <summary>
        /// Finds the closest patrol point to current position
        /// </summary>
        private int FindClosestPatrolPoint()
        {
            int closestIndex = 0;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Brain.PatrolPoints.Length; i++)
            {
                if (Brain.PatrolPoints[i] == null) continue;

                float distance = Vector3.Distance(AITransform.position, Brain.PatrolPoints[i].position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        /// <summary>
        /// Advances to the next patrol point in the loop
        /// </summary>
        private void AdvanceToNextPatrolPoint()
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % Brain.PatrolPoints.Length;
        }

        /// <summary>
        /// Sets the wait time at patrol points
        /// </summary>
        public void SetWaitTime(float waitTime)
        {
            _waitTime = waitTime;
        }

        /// <summary>
        /// Sets the patrol speed multiplier
        /// </summary>
        public void SetPatrolSpeed(float speed)
        {
            _patrolSpeed = Mathf.Clamp01(speed);
        }

        /// <summary>
        /// Gets current patrol point index
        /// </summary>
        public int GetCurrentPatrolIndex()
        {
            return _currentPatrolIndex;
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
    }
}
