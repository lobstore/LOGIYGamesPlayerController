using LOGIYGames.CharacterCore;
using LOGIYGames.Timers;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
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
            _character.RotationStrategy = _character.DefaultRotationStrategy;
            _character.MovementStrategy = _character.DefaultMovementStrategy;
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
    public class WallRunMovementState : BaseMovementState
    {
        Vector3 normal;
        public WallRunMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
        public override void Enter()
        {
            base.Enter();
            _character.RotationStrategy = new ToMoveDirectionRotation(_character);
            _character.MovementStrategy = new WallRunMovement(_character);
            _controller.UseGravity = false;
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            normal = _character.Sensors.IsObstacleLegsRight ? _character.Sensors.LegsRightHit.normal : _character.Sensors.LegsLeftHit.normal;
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _controller.ForceMove(-normal);
        }
        public override void Exit()
        {
            base.Exit();
            _controller.UseGravity = true;
        }
    }
    //public class WallrunState : ContinuousState
    //{
    //    public WallrunState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
    //    {
    //        wallRunGravityMultiplier = 0;
    //        useWallCliping = true;
    //    }
    //    private float wallRunGravityMultiplier;
    //    private bool useWallCliping;
    //    Vector3 normal;
    //    Vector3 magnit => -normal;

    //    public override bool CanBeExecuted()
    //    {
    //        return base.CanBeExecuted()
    //                    && (Sensors.IsObstacleLegsLeft || Sensors.IsObstacleLegsRight)
    //                    && !Sensors.IsGrounded
    //                    && MovementInput.y > 0
    //                    && !Sensors.IsObstacleLegsFront
    //                    && Vector3.Angle(Character.transform.forward, Camera.main.transform.forward) < 60;
    //    }

    //    private void Magnit()
    //    {
    //        CController.Move(magnit * Time.deltaTime);

    //    }
    //    protected override void Rotate()
    //    {
    //        Character.RotateToDirection(moveDirection, TurnSmoothTime);
    //    }
    //    public override void Enter()
    //    {
    //        base.Enter();
    //        CharacterGravity.Velocity = Vector3.zero;
    //        CharacterGravity.UseGravity = false;
    //    }
    //    public override void Exit()
    //    {
    //        base.Exit();
    //        CharacterGravity.UseGravity = true;
    //    }
    //}
}
