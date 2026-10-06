using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using System;
using UnityEngine;
namespace LOGIYGames.Movement
{
    [Serializable]
    public abstract class MovementStateBase : IState
    {
        protected Actor _character;
        protected MovementWrapperBase _controller;
        public MovementStateData Data { get; protected set; }
        protected Animator _animator;
        protected CountdownTimer actionFrameTimer;
        public bool IsActiveState { get; private set; }
        public bool IsActionFrameElapsed => actionFrameTimer.IsFinished;
        public bool IsActionFrameInProgress => actionFrameTimer.IsRunning;

        protected MovementStateBase(Actor ctx, MovementStateData stateData)
        {
            _animator = ctx.GetComponent<Animator>();
            _character = ctx;
            Data = stateData;
            _controller = ctx.GetComponent<MovementWrapperBase>();
            actionFrameTimer = new CountdownTimer(Data.ActionFrameDuration);
        }
        public virtual bool CanEnter() { return true; }
        public virtual bool CanExit() { return true; }
        public virtual void Enter()
        {
            IsActiveState = true;
            Debug.Log("Entered State: " + GetType());
            if (Data.ResetVelocityOnEnter)
            {
                _controller.ResetVelocity();
            }
            if (Data.ResetSpeedOnEnter)
            {
                _character.RuntimeMovement.CurrentSpeed = 0;
            }
            _animator.applyRootMotion = Data.IsAnimationDrivenMovement;
            _character.RuntimeMovement.Acceleration = Data.Acceleration;
            _character.RuntimeMovement.Deceleration = Data.Deceleration;
            _character.RuntimeMovement.TargetSpeed = Data.TargetSpeed;
            _character.RuntimeMovement.TurnSmoothTime = Data.TurnSmoothTime;
            _controller.UseProjectionOnPlane = Data.UseProjectionOnPlane;
            actionFrameTimer.Start();
        }

        public virtual void Exit()
        {
            IsActiveState = false;
            _character.ResetStrategies();
            if (Data.ResetVelocityOnExit)
            {
                _controller.ResetVelocity();
            }
            if (Data.ResetSpeedOnExit)
            {
                _character.RuntimeMovement.CurrentSpeed = 0;
            }
            if (actionFrameTimer.IsRunning)
            {
                actionFrameTimer.Stop();
            }
        }

        public virtual void LogicUpdate()
        {
        }

        public virtual void LateUpdate()
        {
        }
        public virtual void PhysicsUpdate()
        {

        }

    }
}
