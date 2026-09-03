using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class SprintMovementState : MovementStateBase
    {
        public SprintMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData) { }

        public override bool CanExit()
        {
            return !_character.Input.SprintPressing || _character.Input.MovementInput.magnitude == 0;
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Sensors.IsGrounded &&
                        _character.Input.SprintPressing;
        }
    }

}
