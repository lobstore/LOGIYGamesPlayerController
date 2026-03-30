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
        public bool IsActiveState {  get; private set; }
        protected BaseMovementState(Character ctx, MovementStateData stateData)
        {
            _data = new();
            _character = ctx;
            _data.Acceleration = stateData.Acceleration;
            _data.Deceleration = stateData.Deceleration;
            _data.TurnSmoothTime = stateData.TurnSmoothTime;
            _data.Speed = stateData.Speed;
        }

        public virtual void Enter()
        {
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
                if (_character.SpeedMultiplier > 0.01)
                {

                    _character.SpeedMultiplier = Mathf.Lerp(_character.SpeedMultiplier, 0, _character.Deceleration * Time.deltaTime);
                }
                else
                {
                    _character.SpeedMultiplier = 0;
                }
            }
        }

        public virtual void LateUpdate()
        {
           // Aim();

        }

        protected virtual void Aim()
        {
            if (_character.Input.FocusPressed)
            {
                _character.RotationStrategy = new CameraAlongRotation();
            }
            else
            {
                _character.RotationStrategy = _character.DefaultRotationStrategy;
            }
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
