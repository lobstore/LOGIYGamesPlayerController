using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class IdleMovementState : MovementStateBase
    {
        public IdleMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData) { }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Input.MovementInput.magnitude == 0 && _character.Sensors.IsGrounded;
        }
    }
}
