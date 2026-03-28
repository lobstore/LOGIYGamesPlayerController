using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class JumpState : TimedMovementState
    {
        private JumpStateData _stateData;
        public JumpState(Character ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;
        }

        public override void Enter()
        {
            base.Enter();
            _character.EventBus.Publish(new JumpPerformedEvent
            {
                verticalForce = _stateData.VerticalJumpForce,
                planarForce = _stateData.PlanarJumpForce
            });
        }
    }

}
