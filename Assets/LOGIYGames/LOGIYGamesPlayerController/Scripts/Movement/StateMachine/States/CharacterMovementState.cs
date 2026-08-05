using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using System;
using UnityEngine;
namespace LOGIYGames.Movement
{
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
        public virtual bool CanEnter() { return true; }
        public virtual void Enter()
        {
            IsActiveState = true;
            Debug.Log("Entered State: " + GetType());
            if (Data.ResetVelocityOnEnter)
            {
                _character.ResetVelocity();
            }
            _animator.applyRootMotion = Data.IsAnimationDrivenMovement;
            _character.RuntimeMovement.AccelerationData = Data.AccelerationData;
            if (CameraManager.Instance.CurrentCameraPerspectiveType == CameraPerspectiveType.FirstPerson)
            {
                _character.RuntimeMovement.TurnSmoothTime = 0;
            }
            else
            {
            _character.RuntimeMovement.TurnSmoothTime = Data.TurnSmoothTime;

            }
            _character.RuntimeMovement.Speed = Data.Speed;
            _controller.UseProjectionOnPlane = Data.UseProjectionOnPlane;
            actionFrameTimer.Start();
            if (_character.IsGrounded)
            {
                _character.JumpController.JumpCount = 0;
            }
        }

        public virtual void Exit()
        {
            IsActiveState = false;
            _character.ResetStrategies();
            if (Data.ResetVelocityOnExit)
            {
                _character.ResetVelocity();
            }
            if (Data.ResetSpeedOnExit)
            {
                _character.ResetSpeed();
            }
            if (actionFrameTimer.IsRunning)
            {
                actionFrameTimer.Stop();
            }
        }

        public virtual void LogicUpdate()
        {
            Move();
        }

        public virtual void LateUpdate()
        {
            Rotate();
        }
        public virtual void PhysicsUpdate()
        {
        
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
            _character.Rotate(_character.RuntimeMovement.TargetRotation, _character.RuntimeMovement.TurnSmoothTime);
        }
    }
}
