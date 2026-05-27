using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;

namespace LOGIYGames.Movement
{

    public class RollMovementState : TimedMovementState
    {
        private JumpStateData _stateData;

        public RollMovementState(CharacterModule ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;

        }
        public override void Enter()
        {
            base.Enter();
            _character.EventBus.Publish(new JumpPerformedEvent
            {
                jumpType = JumpType.Roll,
                planarForce = _stateData.PlanarJumpForce,
                verticalForce = _stateData.VerticalJumpForce,
            });
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Input.EvadePressed;
        }
    }

}
