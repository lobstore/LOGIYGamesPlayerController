using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using System;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Base interface for all states
    /// </summary>
    public interface IState
    {
        void Enter();
        void Exit();
        void LogicUpdate();
        void LateUpdate();
        void PhysicsUpdate();
    }
    /// <summary>
    /// Abstract base class for states with common functionality
    /// </summary>
    [Serializable]
    public abstract class BaseState : IState
    {
        public IMovementStrategy CurrentMovementStrategy;
        public IRotationStrategy CurrentRotationStrategy;

        protected CameraAlongMovement _cameraAlongMovement;
        protected CameraRelativeRotation _cameraRelativeRotation;
        protected CameraAlongRotation _cameraAlongRotation;

        protected Character _character;
        protected StateData _data;
        protected InputReader _input;

        protected BaseState(MovementStateDriver ctx, StateData stateData)
        {
            _data = new();
            _character = ctx.Character;
            _input = ctx.InputReader;
            _data.StateName = stateData.StateName;
            _data.Acceleration = stateData.Acceleration;
            _data.Deceleration = stateData.Deceleration;
            _data.TurnSmoothTime = stateData.TurnSmoothTime;
            _data.Speed = stateData.Speed;

            _cameraAlongMovement = new(_character);
            _cameraRelativeRotation = new(_character);
            _cameraAlongRotation = new(_character);

            CurrentMovementStrategy = _cameraAlongMovement;
            CurrentRotationStrategy = _cameraRelativeRotation;
        }

        public virtual void Enter()
        {
            _character.Acceleration = _data.Acceleration;
            _character.Deceleration = _data.Deceleration;
            _character.TurnSmoothTime = _data.TurnSmoothTime;
            _character.SpeedMultiplier = _data.Speed;
        }

        public virtual void Exit() { }

        public virtual void LogicUpdate()
        {
            if (_input.BlockPressed || CameraManager.Instance.CameraPerspectiveType == CameraPerspectiveType.FirstPerson)
            {
                CurrentRotationStrategy = _cameraAlongRotation;
            }
            else if (!_input.BlockPressed)
            {
                CurrentRotationStrategy = _cameraRelativeRotation;
            }
        }

        public virtual void LateUpdate() { }

        public virtual void PhysicsUpdate()
        {
            // Get target rotation from rotation strategy
            Quaternion targetRotation = CurrentRotationStrategy.GetRotation();
            
            // Apply rotation through Character (which uses wrapper's rotation system)
            // This ensures proper integration with KinematicCharacterController
            _character.Rotate(targetRotation, _character.TurnSmoothTime);
            
            // Apply movement
            _character.Move(CurrentMovementStrategy.GetMovementDirection());
        }
    }

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

    #region Basic States

    public class IdleState : BaseState
    {
        public IdleState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData) { }
    }

    public class WalkState : BaseState
    {
        public WalkState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData) { }
    }

    public class RunState : BaseState
    {
        public RunState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData) { }
    }

    public class SprintState : BaseState
    {
        public SprintState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData) { }
    }

    public class StopState : BaseState
    {
        public StopState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData) { }
    }

    #endregion

    #region Jump/Fall States

    /// <summary>
    /// Jump state with timer - transitions to Fall only after jump arc completes
    /// </summary>
    public class JumpState : TimedState
    {
        private JumpStateData _stateData;

        public JumpState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;
        }

        public override void Enter()
        {
            base.Enter();
            _character.JumpVerticalForce = _stateData.VerticalJumpForce;
            _character.JumpPlanarForce = _stateData.PlanarJumpForce;
            _character.Jump();
        }
    }

    public class FallingState : BaseState
    {
        public FallingState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData) { }
    }

    public class LandingState : TimedState
    {
        public LandingState(MovementStateDriver ctx, TimedStateData stateData) : base(ctx, stateData) { }
    }

    #endregion

    #region Movement States

    public class CrouchState : BaseState
    {
        protected float StandingHeight;
        protected float CrouchHeight;

        public CrouchState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
        {
            StandingHeight = _character.Height;
            CrouchHeight = StandingHeight * 0.5f;
        }

        public override void Enter()
        {
            base.Enter();
            _character.Height = CrouchHeight;
        }

        public override void Exit()
        {
            base.Exit();
            _character.Height = StandingHeight;
        }
    }

    /// <summary>
    /// Roll state with timer - invincibility frames during roll
    /// </summary>
    public class RollState : TimedState
    {
        private RollStateData _stateData;

        public RollState(MovementStateDriver ctx, RollStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;
        }

        public override void Enter()
        {
            base.Enter();
            _character.JumpVerticalForce = _stateData.VerticalForce;
            _character.JumpPlanarForce = _stateData.PlanarForce;
            _character.Roll();
        }
    }

    #endregion
}
