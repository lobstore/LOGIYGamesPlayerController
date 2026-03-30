using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;
using UnityEngine.UIElements;

namespace LOGIYGames
{
    public class TurnState : TimedMovementState
    {
        public TurnState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData)
        {
        }
        Quaternion turnEnd;
        public override void Enter()
        {
            base.Enter();
            turnEnd = _character.CurrentRotationStrategy.GetRotation();
            _character.EventBus.Publish(new TurnPerformedEvent
            {
                movementSpeed = _character.SpeedMultiplier,
                angle = _character.DeltaYaw
            });
        }
        protected override void Move()
        {
            
        }
        protected override void Rotate()
        {
            _character.Rotate(turnEnd, _character.TurnSmoothTime);
        }
        protected override void Aim()
        {
            
        }
        public override void Exit()
        {
            base.Exit();
            _character.Rotate(turnEnd);
        }
    }
}
