using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class SprintMovementState : MovementStateBase
    {
        public SprintMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData) { }
        protected override void Move()
        {
            if (Data.IsAnimationDrivenMovement) return;
            base.Move();
        }
        public override bool CanExit()
        {
            return !_character.Input.SprintPressing;
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Sensors.IsGrounded &&
                        _character.Input.SprintPressing;
        }
    }

}
