using LOGIYGames;
using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using UnityEngine;
public class TurnMovementState : TimedMovementState
{
    TurnMovementStateData TurnData;
    public TurnMovementState(Character ctx, TurnMovementStateData stateData) : base(ctx, stateData)
    {
        TurnData = stateData;
    }
    Quaternion turnEnd;
    public override void Enter()
    {
        turnEnd = _character.RotationStrategy.GetRotation();
        _character.EventBus.Publish(new TurnPerformedEvent
        {
            movementSpeed = _character.RuntimeMovement.Speed,
            angle = _character.RuntimeMovement.DeltaYaw
        });
        base.Enter();
    }
    protected override void Rotate()
    {
        _character.Rotate(turnEnd, _character.RuntimeMovement.TurnSmoothTime);
    }
    public override bool CanEnter()
    {
        return base.CanEnter() 
            && Mathf.Abs(_character.RuntimeMovement.DeltaYaw) > TurnData.MinAngle 
            && Mathf.Abs(_character.RuntimeMovement.DeltaYaw) < TurnData.MaxAngle 
            && CameraManager.Instance.CurrentCameraPerspectiveType != CameraPerspectiveType.FirstPerson;
    }
}