using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class SprintMovementState : BaseMovementState
    {
        public SprintMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData) { }
        protected override void Move()
        {
            if (_data.IsAnimationDrivenMovement) return;
            base.Move();
        }
    }

}
