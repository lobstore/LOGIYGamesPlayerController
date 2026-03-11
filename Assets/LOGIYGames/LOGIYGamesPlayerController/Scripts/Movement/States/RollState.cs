namespace LOGIYGames.Movement
{

    public class RollState : TimedState
    {
        private JumpStateData _stateData;
        private IRotationStrategy _strategy;

        public RollState(MovementStateDriver ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;
            _strategy = new NoneRotation(_character.transform);

        }
        protected override void ChangeSpeed()
        {
            
        }
        public override void Enter()
        {
            base.Enter();
            _character.CurrentRotationStrategy = _strategy;
            _character.JumpVerticalForce = _stateData.VerticalJumpForce;
            _character.JumpPlanarForce = _stateData.PlanarJumpForce;
            _character.Roll();
            _character.OnRoll.Invoke();
        }
        public override void Exit()
        {
            base.Exit();
            _character.CurrentRotationStrategy = _character.DefaultRotaionStrategy;
        }
    }

}
