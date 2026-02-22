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
        private float _minIdleDuration;
        private float _maxIdleDuration;
        private CountdownTimer _idleTimer;
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

            // Check if target detected - transition to Chase
            if (Brain.Target != null && Brain.HasLineOfSight())
            {
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            AIInput.SetMovementInput(Vector2.zero);
        }

        /// <summary>
        /// Checks if idle timer has completed
        /// </summary>
        public bool IsIdleComplete()
        {
            return _idleTimer != null && _idleTimer.IsFinished;
        }

        /// <summary>
        /// Gets the remaining idle time
        /// </summary>
        public float GetRemainingIdleTime()
        {
            return _idleTimer?.CurrentTime ?? 0f;
        }

        /// <summary>
        /// Sets the min and max idle duration range
        /// </summary>
        public void SetIdleDurationRange(float min, float max)
        {
            _minIdleDuration = Mathf.Min(min, max);
            _maxIdleDuration = Mathf.Max(min, max);
        }
    }
}
