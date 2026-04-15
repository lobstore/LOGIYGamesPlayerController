using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames.Movement
{
    public class StopMovementState : TimedMovementState
    {
        public StopMovementState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData) { }
        public override void Enter()
        {
            base.Enter();
            Debug.Log(_character.targetDirection);
            Direction dir = _character.GetRelativeMovementDirection();
            _character.EventBus.Publish(new MovementStoppedEvent
            {
                direction = dir,
                speed = _character.SpeedMultiplier
            });
            _character.MovementStrategy = new NoneMovement();
            //_character.RotationStrategy = new NoneRotation(_character.transform);
        }
        protected override void Rotate()
        {
            
        }
        public override void Exit()
        {
            base.Exit();
            _character.MovementStrategy = _character.DefaultMovementStrategy;
            //_character.RotationStrategy = _character.DefaultRotationStrategy;
        }
    }

}
