using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class IdleMovementState : CharacterMovementState
    {
        public IdleMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData) { }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Input.MovementInput.magnitude == 0 && _character.Sensors.IsGrounded;
        }
    }
}
