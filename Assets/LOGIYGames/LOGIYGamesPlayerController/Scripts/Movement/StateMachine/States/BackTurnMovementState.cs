using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using UnityEngine;

namespace LOGIYGames
{
    public class BackTurnMovementState : TimedMovementState
    {
        public BackTurnMovementState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData)
        {
        }
        Quaternion turnEnd;
        public override void Enter()
        {
            base.Enter();
            turnEnd = _character.RotationStrategy.GetRotation();
            _character.EventBus.Publish(new TurnPerformedEvent
            {
                movementSpeed = _character.SpeedMultiplier,
                angle = _character.DeltaYaw
            });
            _character.ResetVelocity();
        }
        protected override void Move()
        {

        }
        protected override void Rotate()
        {
            if (_data.IsAnimationDriven) return;
            _character.Rotate(turnEnd, _character.TurnSmoothTime);
        }
        public override void Exit()
        {
            base.Exit();
            if (_data.IsAnimationDriven) return;
            _character.Rotate(turnEnd);
        }
    }

}
