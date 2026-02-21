namespace LOGIYGames.Movement
{


    /// <summary>
    /// Jump state with timer - transitions to Fall only after jump arc completes
    /// </summary>
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
        }
    }

}
