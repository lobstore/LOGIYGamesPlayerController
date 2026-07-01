using LOGIYGames;
using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using UnityEngine;

public class TurnMovementState : TimedMovementState
{
    public TurnMovementState(Character ctx, TurnMovementStateData stateData) : base(ctx, stateData)
    {
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
}