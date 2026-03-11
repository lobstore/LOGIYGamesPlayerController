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
        protected BaseMovementState(MovementStateDriver ctx, MovementStateData stateData)
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

            _character.CurrentMovementStrategy = _character.DefaultMovementStrategy;
            _character.CurrentRotationStrategy = _character.DefaultRotaionStrategy;
            _character.Acceleration = _data.Acceleration;
            _character.Deceleration = _data.Deceleration;
            _character.TurnSmoothTime = _data.TurnSmoothTime;
        }

        public virtual void Exit()
        {
            _character.JumpPlanarForce = 0;
            _character.JumpVerticalForce = 0;
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
        }



        public virtual void PhysicsUpdate()
        {
            Quaternion targetRotation = _character.CurrentRotationStrategy.GetRotation();
            _character.Rotate(targetRotation, _character.TurnSmoothTime);

            Vector3 dir = _character.CurrentMovementStrategy.GetMovementDirection();
            _character.Move(dir);
        }
    }

}
