namespace LOGIYGames.Movement
{
    public class JumpState : TimedState
    {
        private JumpStateData _stateData;

        public JumpState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;
        }

        public override void Enter()
        {
            base.Enter();
            _character.JumpVerticalForce = _stateData.VerticalJumpForce;
            _character.JumpPlanarForce = _stateData.PlanarJumpForce;
            _character.Jump();
            _character.OnJump.Invoke(true);
        }
        public override void Exit()
        {
            base.Exit();
            _character.OnJump.Invoke(false);
        }
    }

}
