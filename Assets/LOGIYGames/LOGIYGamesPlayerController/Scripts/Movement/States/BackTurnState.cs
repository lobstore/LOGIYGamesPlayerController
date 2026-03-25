using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public class BackTurnState : TimedMovementState
    {
        public BackTurnState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _character.CurrentRotationStrategy = new NoneRotation(_character.transform);
            _character.CurrentMovementStrategy = new NoneMovement();
            _character.EventBus.Publish(new TurnPerformedEvent
            {
                speed = _character.SpeedMultiplier,
                angle = _character.DeltaYaw
            });
        }
        protected override void Aim()
        {
            
        }
        public override void Exit()
        {
            base.Exit();
            _character.CurrentRotationStrategy = _character.DefaultRotaionStrategy;
            _character.CurrentMovementStrategy = _character.DefaultMovementStrategy;
        }
    }
}
