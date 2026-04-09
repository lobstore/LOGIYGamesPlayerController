using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;

namespace LOGIYGames.Movement
{

    public class RollMovementState : TimedMovementState
    {
        private JumpStateData _stateData;

        public RollMovementState(Character ctx, JumpStateData stateData) : base(ctx, stateData)
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
            _character.MovementStrategy = new NoneMovement();
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
