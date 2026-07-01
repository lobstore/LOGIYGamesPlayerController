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
            Direction dir = _character.GetRelativeMovementDirection();
            _character.EventBus.Publish(new MovementStoppedEvent
            {
                movementSpeed = _character.RuntimeMovement.Speed,
                direction = dir,
            });
            base.Enter();
        }
        public override void Exit()
        {
            base.Exit();
            _character.MovementStrategy = _character.DefaultMovementStrategy;
        }
    }

}
