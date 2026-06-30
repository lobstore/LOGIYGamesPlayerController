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
    }

}
