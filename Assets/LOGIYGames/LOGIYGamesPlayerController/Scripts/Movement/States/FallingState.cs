namespace LOGIYGames.Movement
{
    public class FallingState : BaseMovementState
    {
        public FallingState(MovementStateDriver ctx, MovementStateData stateData) : base(ctx, stateData) { }

        public override void Enter()
        {
            base.Enter();
            _character.IsFalling = true;
        }
        public override void Exit()
        {
            base.Exit();
            _character.IsFalling = false;
        }
    }

}
