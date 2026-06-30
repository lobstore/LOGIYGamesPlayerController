using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using System;
using UnityEngine;
namespace LOGIYGames.Movement
{
    /// <summary>
    /// Abstract base class for states with common functionality
    /// </summary>
    [Serializable]
    public abstract class CharacterMovementState : IState
    {
        protected Character _character;
        protected MovementWrapperBase _controller;
        public MovementStateData Data { get; protected set; }
        protected Animator _animator;
        protected CountdownTimer actionFrameTimer;
        public bool IsActiveState { get; private set; }
        public bool IsActionFrameElapsed => actionFrameTimer.IsFinished;
        public bool IsActionFrameInProgress => actionFrameTimer.IsRunning;

        protected CharacterMovementState(Character ctx, MovementStateData stateData)
        {
            _animator = ctx.GetComponent<Animator>();
            _character = ctx;
            Data = stateData;
            _controller = ctx.GetComponent<MovementWrapperBase>();
            actionFrameTimer = new CountdownTimer(Data.ActionFrameDuration);
        }
        public virtual void Enter()
        {
            IsActiveState = true;
            Debug.Log("Entered State: " + GetType());
            if (Data.ResetVelocityOnEnter)
            {
                _character.ResetVelocity();
            }
            _animator.applyRootMotion = Data.IsAnimationDrivenMovement;
            _character.AccelerationData = Data.AccelerationData;
            _character.TurnSmoothTime = Data.TurnSmoothTime;
            _controller.UseProjectionOnPlane = Data.UseProjectionOnPlane;
            actionFrameTimer.Start();
            if (_character.IsGrounded)
            {
                _character.JumpCount = 0;
            }
        }

        public virtual void Exit()
        {
            IsActiveState = false;
            if (Data.ResetVelocityOnExit)
            {
                _character.ResetVelocity();
            }
            if (Data.ResetSpeedOnExit)
            {
                _character.ResetSpeed();
            }
            _character.ResetStrategies();
            if (actionFrameTimer.IsRunning)
            {
                actionFrameTimer.Stop();
            }
        }

        public virtual void LogicUpdate()
        {
            UpdateSpeed();
            Rotate();
        }

        public virtual void LateUpdate()
        {

        }
        public virtual void PhysicsUpdate()
        {
            Move();
        }
        protected virtual void Move()
        {
            if (Data.IsAnimationDrivenMovement)
            {
                return;
            }
            _character.Move();
        }
        protected virtual void Rotate()
        {
            if (Data.IsAnimationDrivenRotation)
            {
                return;
            }
            _character.Rotate(_character.TargetRotation, _character.TurnSmoothTime);
        }
        /// <summary>
        /// Smooth change speed for animations purpose
        /// </summary>
        private void UpdateSpeed()
        {
            if (_character.Input.MovementInput.magnitude > 0)
            {
                _character.Speed = Mathf.Lerp(_character.Speed, Data.Speed, Time.deltaTime * _character.AccelerationData.Acceleration);

            }
            else
            {
                _character.Speed = Mathf.Lerp(_character.Speed, 0, Time.deltaTime * _character.AccelerationData.Deceleration);
            }
        }
    }
}
