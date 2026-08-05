using LOGIYGames;
using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;

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
    public override bool CanEnter()
    {
        return base.CanEnter() 
            && _character.Input.MovementInput.magnitude == 0 
            && CameraManager.Instance.CurrentCameraPerspectiveType != CameraPerspectiveType.FirstPerson;
    }
}

