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
    public abstract class BaseMovementState : IState
    {
        protected Character _character;
        protected ControllerWrapperBase _controller;
        protected MovementStateData _data;
        protected Animator _animator;
        protected CountdownTimer actionFrameTimer;
        public bool IsActiveState { get; private set; }
        public bool IsActionFrameElapsed => actionFrameTimer.IsFinished;
        public bool IsActionFrameInProgress => actionFrameTimer.IsRunning;

        protected BaseMovementState(Character ctx, MovementStateData stateData)
        {
            _animator = ctx.GetComponent<Animator>();
            _character = ctx;
            _data = stateData;
            _controller = ctx.GetComponent<ControllerWrapperBase>();
            actionFrameTimer = new CountdownTimer(_data.ActionFrameDuration);
        }

        public virtual void Enter()
        {
            //Debug.Log("Entered State" + GetType());
            _animator.applyRootMotion = _data.IsAnimationDrivenMovement;
            _character.Acceleration = _data.Acceleration;
            _character.Deceleration = _data.Deceleration;
            _character.TurnSmoothTime = _data.TurnSmoothTime;
            _controller.UseProjectionOnPlane = _data.UseProjectionOnPlane;
            actionFrameTimer.Start();
            IsActiveState = true;
            if (_character.IsGrounded)
            {
                _character.JumpCount = 0;
            }
        }

        public virtual void Exit()
        {
            IsActiveState = false;
            if (actionFrameTimer.IsRunning)
            {
                actionFrameTimer.Stop();

            }
        }

        public virtual void LogicUpdate()
        {
            ChangeSpeed();
        }

        protected virtual void ChangeSpeed()
        {
            if (_character.Input.MovementInput.magnitude > 0)
            {
                _character.SpeedMultiplier = Mathf.Lerp(_character.SpeedMultiplier, _data.Speed, _character.Acceleration * Time.deltaTime);
            }
            else
            {
                _character.SpeedMultiplier = Mathf.Lerp(_character.SpeedMultiplier, 0, _character.Deceleration * Time.deltaTime);
            }
        }

        public virtual void LateUpdate()
        {

        }


        public virtual void PhysicsUpdate()
        {
            Move();
            Rotate();
        }
        protected virtual void Move()
        {
            if (_data.IsAnimationDrivenMovement)
            {
                return;
            }
            _character.Move(_character.targetDirection);
        }
        protected virtual void Rotate()
        {
            if (_data.IsAnimationDrivenRotation)
            {
                return;
            }
            _character.Rotate(_character.targetRotation, _character.TurnSmoothTime);
        }
    }

}
