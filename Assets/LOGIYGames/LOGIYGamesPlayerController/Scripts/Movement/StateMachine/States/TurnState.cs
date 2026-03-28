using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public class TurnState : TimedMovementState
    {
        public TurnState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _character.EventBus.Publish(new TurnPerformedEvent
            {
                movementSpeed = _character.SpeedMultiplier,
                angle = _character.DeltaYaw
            });
        }
        protected override void Rotate()
        {
            
        }
        protected override void Move()
        {
            
        }
        protected override void Aim()
        {
            
        }
        public override void Exit()
        {
            base.Exit();
        }
    }
}
