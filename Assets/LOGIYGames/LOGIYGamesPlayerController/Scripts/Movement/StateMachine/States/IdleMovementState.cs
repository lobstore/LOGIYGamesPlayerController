using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class IdleMovementState : BaseMovementState
    {
        public IdleMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData) { }
        public override void Enter()
        {
            base.Enter();
            _character.ResetVelocity();
        }
        protected override void Rotate()
        {
            
        }
    }
}
