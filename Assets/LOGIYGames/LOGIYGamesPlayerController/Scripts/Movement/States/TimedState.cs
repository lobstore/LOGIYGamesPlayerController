using LOGIYGames.Timers;
using UnityEngine;

namespace LOGIYGames.Movement
{
    /// <summary>
    /// Base state with timer/cooldown support using CountdownTimer
    /// Supports both duration (minimum time in state) and cooldown (delay before re-entry)
    /// </summary>
    public abstract class TimedState : BaseState
    {
        protected CountdownTimer _durationTimer;
        protected CountdownTimer _cooldownTimer;

        protected TimedState(MovementStateDriver ctx, TimedStateData stateData) : base(ctx, stateData)
        {
            // Create timers from data
            if (stateData.Duration > 0)
            {
                _durationTimer = new CountdownTimer(stateData.Duration);
            }

            if (stateData.Cooldown > 0)
            {
                _cooldownTimer = new CountdownTimer(stateData.Cooldown);
            }
        }

        public override void Enter()
        {
            base.Enter();

            // Check cooldown before entering
            if (!CanEnter())
            {
                Debug.LogWarning($"Cannot enter {GetType().Name} - cooldown is still running");
                return;
            }

            // Start duration timer
            if (_durationTimer != null)
            {
                _durationTimer.Start();
            }
        }

        public override void Exit()
        {
            base.Exit();

            // Stop duration timer
            if (_durationTimer != null)
            {
                _durationTimer.Stop();
            }

            // Start cooldown timer
            if (_cooldownTimer != null)
            {
                _cooldownTimer.Start();
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }

        /// <summary>
        /// Check if state can be entered (cooldown check)
        /// </summary>
        public virtual bool CanEnter()
        {
            if (_cooldownTimer != null)
            {
                return _cooldownTimer.IsFinished;
            }
            return true;
        }
        public bool IsDurationTimerElapsed => _durationTimer?.IsFinished == true;
        public bool IsCooldownTimerElapsed => _cooldownTimer?.IsFinished == true;
        public bool IsDurationTimerRunning => _durationTimer?.IsRunning == true;
        public bool IsCooldownTimerRunning => _cooldownTimer?.IsRunning == true;
        public float DurationTimerProgress => _durationTimer?.Progress ?? 0f;
        public float CooldownTimerProgress => _cooldownTimer?.Progress ?? 0f;
        public float DurationTimerRemaining => _durationTimer?.CurrentTime ?? 0f;
        public float CooldownTimerRemaining => _cooldownTimer?.CurrentTime ?? 0f;
    }

}
