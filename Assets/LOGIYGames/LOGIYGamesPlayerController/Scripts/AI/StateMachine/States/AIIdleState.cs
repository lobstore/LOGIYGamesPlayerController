using LOGIYGames.Timers;
using UnityEngine;

namespace LOGIYGames.AI
{
    /// <summary>
    /// AI Idle state - AI stays in place for a random duration
    /// </summary>
    public class AIIdleState : AIBaseState
    {
        private readonly float _minIdleDuration;
        private readonly float _maxIdleDuration;
        private CountdownTimer _idleTimer;
        private float _currentIdleDuration;

        public AIIdleState(AIBrainStateDriver brain, float minIdleDuration = 2f, float maxIdleDuration = 5f) : base(brain)
        {
            _minIdleDuration = minIdleDuration;
            _maxIdleDuration = maxIdleDuration;
            DetectionRange = brain.DetectionRange;
            AttackRange = brain.AttackRange;
        }

        public override void Enter()
        {
            base.Enter();
            Stop();

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

        /// <summary>
        /// Checks if idle timer has completed
        /// </summary>
        public bool IsIdleComplete()
        {
            return _idleTimer != null && _idleTimer.IsFinished;
        }
    }
}
