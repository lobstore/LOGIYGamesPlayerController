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
        public IMovementStrategy CurrentMovementStrategy;
        public IRotationStrategy CurrentRotationStrategy;

        protected CameraAlongMovement _cameraAlongMovement;
        protected CameraRelativeRotation _cameraRelativeRotation;
        protected CameraAlongRotation _cameraAlongRotation;

        protected Character _character;
        protected StateData _data;

        protected BaseState(MovementStateDriver ctx, StateData stateData)
        {
            _data = new();
            _character = ctx.Character;
            _data.StateName = stateData.StateName;
            _data.Acceleration = stateData.Acceleration;
            _data.Deceleration = stateData.Deceleration;
            _data.TurnSmoothTime = stateData.TurnSmoothTime;
            _data.Speed = stateData.Speed;

            _cameraAlongMovement = new(_character);
            _cameraRelativeRotation = new(_character);
            _cameraAlongRotation = new(_character);

            CurrentMovementStrategy = _cameraAlongMovement;
            CurrentRotationStrategy = _cameraRelativeRotation;
        }

        public virtual void Enter()
        {
            _character.Acceleration = _data.Acceleration;
            _character.Deceleration = _data.Deceleration;
            _character.TurnSmoothTime = _data.TurnSmoothTime;
            _character.SpeedMultiplier = _data.Speed;
        }

        public virtual void Exit() { }

        public virtual void LogicUpdate()
        {
        }

        public virtual void LateUpdate() { }

        public virtual void PhysicsUpdate()
        {
            // Get target rotation from rotation strategy
            Quaternion targetRotation = CurrentRotationStrategy.GetRotation();
            _character.Rotate(targetRotation, _character.TurnSmoothTime);

            // Apply movement
            Vector3 dir = CurrentMovementStrategy.GetMovementDirection();
            _character.Move(dir);
        }
    }

}
