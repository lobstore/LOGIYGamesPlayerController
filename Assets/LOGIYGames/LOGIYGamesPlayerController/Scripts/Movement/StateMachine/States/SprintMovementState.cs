using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class SprintMovementState : CharacterMovementState
    {
        public SprintMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData) { }
        protected override void Move()
        {
            if (Data.IsAnimationDrivenMovement) return;
            base.Move();
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Sensors.IsGrounded &&
                        _character.Input.SprintPressing;
        }
    }

}
