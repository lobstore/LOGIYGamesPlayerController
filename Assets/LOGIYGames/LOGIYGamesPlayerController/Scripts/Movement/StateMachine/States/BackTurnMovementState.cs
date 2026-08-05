using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using UnityEngine;

namespace LOGIYGames
{
    public class BackTurnMovementState : TimedMovementState
    {
        TurnMovementStateData TurnData;
        public BackTurnMovementState(Character ctx, TurnMovementStateData stateData) : base(ctx, stateData)
        {
            TurnData = stateData;
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
        public override bool CanEnter()
        {
            return base.CanEnter() 
                && Mathf.Abs(_character.RuntimeMovement.DeltaYaw) > TurnData.MinAngle 
                && CameraManager.Instance.CurrentCameraPerspectiveType != CameraPerspectiveType.FirstPerson;
        }
    }

}
