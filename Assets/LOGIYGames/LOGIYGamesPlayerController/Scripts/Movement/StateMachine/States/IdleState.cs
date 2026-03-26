using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class IdleState : BaseMovementState
    {
        public IdleState(Character ctx, MovementStateData stateData) : base(ctx, stateData) { }
        public override void Enter()
        {
            base.Enter();
            _character.JumpCount = 0;
        }
    }
}
