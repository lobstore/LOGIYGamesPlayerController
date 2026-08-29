using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;

namespace LOGIYGames.Movement
{
    /// <summary>
    /// Base state with timer/cooldown support using CountdownTimer
    /// Supports both duration (minimum time in state) and cooldown (delay before re-entry)
    /// </summary>
    public abstract class TimedMovementState : MovementStateBase
    {
        protected CountdownTimer _durationTimer;
        protected CountdownTimer _cooldownTimer;

        protected TimedMovementState(Actor ctx, TimedMovementStateData stateData) : base(ctx, stateData)
        {
                _durationTimer = new CountdownTimer(stateData.Duration);


            if (stateData.Cooldown > 0)
            {
                _cooldownTimer = new CountdownTimer(stateData.Cooldown);
            }
        }

        public override void Enter()
        {
            base.Enter();

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
        public override bool CanEnter()
        {
            if (_cooldownTimer != null)
            {
                return base.CanEnter()&&!_cooldownTimer.IsRunning;
            }
            else
            {
                return base.CanEnter();

            }
        }
        public override bool CanExit()
        {
            return IsDurationTimerElapsed;
        }
        public bool IsDurationTimerElapsed => _durationTimer?.IsFinished == true;
        public bool IsCooldownTimerElapsed => _cooldownTimer?.IsFinished == true;
        public bool IsDurationTimerRunning => _durationTimer?.IsRunning == true;
        public bool IsCooldownTimerRunning => _cooldownTimer?.IsRunning == true;
        public float DurationTimerProgress => _durationTimer?.Progress ?? 0f;
        public float CooldownTimerProgress => _cooldownTimer?.Progress ?? 0f;
        public float DurationTimerRemaining => _durationTimer?.CurrentTime.CurrentValue ?? 0f;
        public float CooldownTimerRemaining => _cooldownTimer?.CurrentTime.CurrentValue ?? 0f;
    }

}
