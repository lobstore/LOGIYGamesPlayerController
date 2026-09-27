using LOGIYGames;
using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;

public class StopMovementState : TimedMovementState
{
    public StopMovementState(Actor ctx, TimedMovementStateData stateData) : base(ctx, stateData) { }
    public override void Enter()
    {
        Direction dir = _character.GetRelativeMovementDirection();
        _character.EventBus.Publish(new MovementStoppedEvent
        {
            movementSpeed = _character.RuntimeMovement.CurrentSpeed,
            direction = dir,
        });
        base.Enter();
    }
    public override bool CanEnter()
    {
        return base.CanEnter() 
            && _character.Input.MovementInput.magnitude == 0 
            && _character.RuntimeMovement.CurrentSpeed > 0.1
            && _character.DefaultMovementStrategy is not StrafeMovement;
    }
}

