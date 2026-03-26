using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{

    public class RollState : TimedMovementState
    {
        private JumpStateData _stateData;
        private IRotationStrategy _strategy;

        public RollState(Character ctx, JumpStateData stateData) : base(ctx, stateData)
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
            _character.EventBus.Publish(new RollPerformedEvent
            {
                planarForce = _stateData.PlanarJumpForce,
                verticalForce = _stateData.VerticalJumpForce,
            });
        }
        protected override void Aim()
        {
            
        }
        public override void Exit()
        {
            base.Exit();
            _character.CurrentRotationStrategy = _character.DefaultRotaionStrategy;
        }
    }

}
