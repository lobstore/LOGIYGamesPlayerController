using LOGIYGames.CharacterCore;
using System;
using UnityEngine;
using UnityEngine.Events;

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
        // Что планируется
        //public readonly UnityEvent OnStateEnter = new();
        //public readonly UnityEvent OnStateExit = new();
        //public readonly UnityEvent OnStateUpdate = new();
        //public readonly UnityEvent OnStateFixedUpdate = new();
        //public readonly UnityEvent OnStateLateUpdate = new();
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
            //OnStateEnter.Invoke();
            _character.JumpPlanarForce = 0;
            _character.JumpVerticalForce = 0;
            _character.CurrentMovementStrategy = _character.DefaultMovementStrategy;
            _character.CurrentRotationStrategy = _character.DefaultRotaionStrategy;
            _character.Acceleration = _data.Acceleration;
            _character.Deceleration = _data.Deceleration;
            _character.TurnSmoothTime = _data.TurnSmoothTime;
            _character.SpeedMultiplier = _data.Speed;
        }

        public virtual void Exit() 
        {
            //OnStateExit.Invoke();
        }

        public virtual void LogicUpdate()
        {
            //OnStateUpdate.Invoke();
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
            //OnStateLateUpdate.Invoke(); 
        }

        public virtual void PhysicsUpdate()
        {
            //OnStateFixedUpdate.Invoke();
            // Get target rotation from rotation strategy
            Quaternion targetRotation = _character.CurrentRotationStrategy.GetRotation();
            _character.Rotate(targetRotation, _character.TurnSmoothTime);

            // Apply movement
            Vector3 dir = _character.CurrentMovementStrategy.GetMovementDirection();
            _character.Move(dir);
        }
    }

}
