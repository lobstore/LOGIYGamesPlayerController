using LOGIYGames.CharacterCore;
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
        protected MovementStateData _data;
        protected Animator _animator;
        public bool IsActiveState { get; private set; }
        protected BaseMovementState(Character ctx, MovementStateData stateData)
        {
            _animator = ctx.GetComponent<Animator>();
            _data = new();
            _character = ctx;
            _data = stateData;
        }

        public virtual void Enter()
        {
            //Debug.Log("Entered State" + GetType());
            _animator.applyRootMotion = _data.IsAnimationDriven;
            _character.Acceleration = _data.Acceleration;
            _character.Deceleration = _data.Deceleration;
            _character.TurnSmoothTime = _data.TurnSmoothTime;
            IsActiveState = true;
        }

        public virtual void Exit()
        {
            IsActiveState = false;
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
            _character.Move(_character.targetDirection);
        }
        protected virtual void Rotate()
        {
            _character.Rotate(_character.targetRotation, _character.TurnSmoothTime);
        }
    }

}
