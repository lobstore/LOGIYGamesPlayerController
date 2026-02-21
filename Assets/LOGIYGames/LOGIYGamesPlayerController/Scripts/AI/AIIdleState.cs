using LOGIYGames.Timers;
using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Idle state - AI stays in place for a random duration
    /// Transitions to Patrol when idle timer completes
    /// </summary>
    public class AIIdleState : AIBaseState
    {
        /// <summary>
        /// Min and max idle duration before switching to patrol
        /// </summary>
        private float _minIdleDuration = 2f;
        private float _maxIdleDuration = 5f;

        /// <summary>
        /// Timer for idle duration
        /// </summary>
        private CountdownTimer _idleTimer;

        /// <summary>
        /// Current idle duration (randomized on each enter)
        /// </summary>
        private float _currentIdleDuration;

        public AIIdleState(AIBrain brain, float minIdleDuration = 2f, float maxIdleDuration = 5f) : base(brain)
        {
            _minIdleDuration = minIdleDuration;
            _maxIdleDuration = maxIdleDuration;
            DetectionRange = brain.DetectionRange;
            AttackRange = brain.AttackRange;
        }

        public override void Enter()
        {
            base.Enter();
            AIInput.ClearAllInputs();
            
            // Randomize idle duration
            _currentIdleDuration = Random.Range(_minIdleDuration, _maxIdleDuration);
            
            // Setup and start idle timer
            _idleTimer?.Dispose();
            _idleTimer = new CountdownTimer(_currentIdleDuration);
            _idleTimer.Start();
        }

        public override void Exit()
        {
            base.Exit();
            _idleTimer?.Stop();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Check if target is detected - transition to Chase
            if (Brain.Target != null && CanDetectTarget())
            {
                return;
            }

            // Check if idle timer completed - ready to transition to Patrol
            if (_idleTimer != null && _idleTimer.IsFinished)
            {
                // Transition handled by AIBrain predicates
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            // No movement in idle state
            AIInput.SetMovementInput(Vector2.zero);
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
        /// Gets the remaining idle time
        /// </summary>
        public float GetRemainingIdleTime()
        {
            return _idleTimer?.CurrentTime ?? 0f;
        }

        /// <summary>
        /// Gets the current idle duration
        /// </summary>
        public float GetCurrentIdleDuration()
        {
            return _currentIdleDuration;
        }

        /// <summary>
        /// Sets the min and max idle duration range
        /// </summary>
        public void SetIdleDurationRange(float min, float max)
        {
            _minIdleDuration = Mathf.Min(min, max);
            _maxIdleDuration = Mathf.Max(min, max);
        }

        /// <summary>
        /// Checks if idle timer has completed
        /// </summary>
        public bool IsIdleComplete()
        {
            return _idleTimer != null && _idleTimer.IsFinished;
        }
    }
}
