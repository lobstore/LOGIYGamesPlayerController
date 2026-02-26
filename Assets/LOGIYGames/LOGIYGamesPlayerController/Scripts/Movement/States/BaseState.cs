using LOGIYGames.CharacterCore;
using System;
using UnityEngine;

namespace LOGIYGames.Movement
{
    /// <summary>
    /// Abstract base class for states with common functionality
    /// </summary>
    [Serializable]
    public abstract class BaseState : IState
    {
        protected Character _character;
        protected MovementStateData _data;

        protected BaseState(MovementStateDriver ctx, MovementStateData stateData)
        {
            _data = new();
            _character = ctx.Character;
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
            _character.SpeedMultiplier = _data.Speed;
            if (_character.CurrentRotationStrategy == null)
            {
                _character.CurrentRotationStrategy = new NoneRotation(_character.transform);

            }
            if (_character.CurrentMovementStrategy == null)
            {
                _character.CurrentMovementStrategy = new NoneMovement();
            }
        }

        public virtual void Exit() { }

        public virtual void LogicUpdate()
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

        public virtual void LateUpdate() { }

        public virtual void PhysicsUpdate()
        {
            // Get target rotation from rotation strategy
            Quaternion targetRotation = _character.CurrentRotationStrategy.GetRotation();
            _character.Rotate(targetRotation, _character.TurnSmoothTime);

            // Apply movement
            Vector3 dir = _character.CurrentMovementStrategy.GetMovementDirection();
            _character.Move(dir);
        }
    }

}
