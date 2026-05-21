using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;

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
            _character.EventBus.Publish(new JumpPerformedEvent
            {
                jumpType = JumpType.Slip,
                planarForce = _stateData.PlanarJumpForce,
                verticalForce = _stateData.VerticalJumpForce,
            });
        }

    }

}
