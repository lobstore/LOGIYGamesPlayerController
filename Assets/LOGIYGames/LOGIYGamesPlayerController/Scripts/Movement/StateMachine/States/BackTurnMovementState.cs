using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using UnityEngine;

namespace LOGIYGames
{
    public class BackTurnMovementState : TimedMovementState
    {
        public BackTurnMovementState(Character ctx, TurnMovementStateData stateData) : base(ctx, stateData)
        {
        }
        Quaternion turnEnd;
        public override void Enter()
        {
            turnEnd = _character.RotationStrategy.GetRotation();
            _character.EventBus.Publish(new BackTurnPerformedEvent
            {
                movementSpeed = _character.RuntimeMovement.Speed,
                angle = _character.RuntimeMovement.DeltaYaw
            });
            base.Enter();
        }
        protected override void Rotate()
        {
            if (Data.IsAnimationDrivenRotation) return;
            _character.Rotate(turnEnd, _character.RuntimeMovement.TurnSmoothTime);
        }
        public override void Exit()
        {
            base.Exit();
            if (Data.IsAnimationDrivenRotation) return;
            _character.Rotate(turnEnd);
        }
    }

}
