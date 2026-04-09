using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;

namespace LOGIYGames
{
    public class SlipMovementState : TimedMovementState
    {
        private JumpStateData _stateData;
        public SlipMovementState(Character ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;
        }
        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new NoneMovement();
            _character.EventBus.Publish(new SlipPerformedEvent()
            {
                planarForce = _stateData.PlanarJumpForce,
                verticalForce = _stateData.VerticalJumpForce,
            });
        }
        protected override void Rotate()
        {

        }
        public override void Exit()
        {
            base.Exit();
            _character.MovementStrategy = _character.DefaultMovementStrategy;
        }

    }

}
