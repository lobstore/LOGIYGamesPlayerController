namespace LOGIYGames.Movement
{

    /// <summary>
    /// Roll state with timer - invincibility frames during roll
    /// </summary>
    public class RollState : TimedState
    {
        private JumpStateData _stateData;

        public RollState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;
        }
        IRotationStrategy prev;
        public override void Enter()
        {
            base.Enter();
            _character.JumpVerticalForce = _stateData.VerticalJumpForce;
            _character.JumpPlanarForce = _stateData.PlanarJumpForce;
            _character.Roll();
            prev = _character.CurrentRotationStrategy;
            _character.CurrentRotationStrategy = new NoneRotation(_character.transform);
            _character.OnRoll.Invoke(true);
        }
        public override void Exit()
        {
            base.Exit();
            _character.OnRoll.Invoke(false);
            _character.CurrentRotationStrategy = prev;
        }
    }

}
