namespace LOGIYGames.Movement
{

    /// <summary>
    /// Roll state with timer - invincibility frames during roll
    /// </summary>
    public class RollState : TimedState
    {
        private RollStateData _stateData;

        public RollState(MovementStateDriver ctx, RollStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;
        }

        public override void Enter()
        {
            base.Enter();
            _character.JumpVerticalForce = _stateData.VerticalForce;
            _character.JumpPlanarForce = _stateData.PlanarForce;
            _character.Roll();
        }
    }

}
