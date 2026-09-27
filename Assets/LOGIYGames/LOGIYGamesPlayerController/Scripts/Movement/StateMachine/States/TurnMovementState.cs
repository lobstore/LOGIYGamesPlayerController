using LOGIYGames;
using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using UnityEngine;
public class TurnMovementState : TimedMovementState
{
    LocomotionController controller;
    TurnMovementStateData TurnData;
    Quaternion targetRotation;
    public TurnMovementState(Actor ctx, TurnMovementStateData stateData) : base(ctx, stateData)
    {
        controller = ctx.GetComponent<LocomotionController>();
        TurnData = stateData;
    }
    public override void Enter()
    {
        _character.MovementStrategy = new CharacterForwardMovement(_character);
        _character.EventBus.Publish(new TurnPerformedEvent
        {
            movementSpeed = _character.RuntimeMovement.CurrentSpeed,
            angle = _character.RuntimeMovement.DeltaYaw
        });
        targetRotation = _character.RuntimeMovement.TargetRotation;
        base.Enter();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (!Data.IsAnimationDrivenMovement)
        {
            controller.Move();

        }
        if (!Data.IsAnimationDrivenRotation)
        {
            _character.Rotate(targetRotation);

        }
    }

    public override bool CanEnter()
    {
        return base.CanEnter()
            && Mathf.Abs(_character.RuntimeMovement.DeltaYaw) > TurnData.MinAngle
            && Mathf.Abs(_character.RuntimeMovement.DeltaYaw) < TurnData.MaxAngle
            && _character.DefaultMovementStrategy is not StrafeMovement;
    }
}