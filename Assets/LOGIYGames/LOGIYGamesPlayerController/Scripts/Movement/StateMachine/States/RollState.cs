using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{

    public class RollState : TimedMovementState
    {
        private JumpStateData _stateData;

        public RollState(Character ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;

        }
        public override void Enter()
        {
            base.Enter();
            _character.EventBus.Publish(new RollPerformedEvent
            {
                planarForce = _stateData.PlanarJumpForce,
                verticalForce = _stateData.VerticalJumpForce,
            });
        }
        protected override void Rotate()
        {
            
        }
    }

}
