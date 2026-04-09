using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class RunMovementState : BaseMovementState
    {
        public RunMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData) { }
        protected override void Move()
        {
            if (_data.IsAnimationDriven)
            {
                return;
            }
            base.Move();
        }
    }
    
}
