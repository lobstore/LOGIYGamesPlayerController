using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;

namespace LOGIYGames
{
    public class DashMovementState : TimedMovementState
    {
        private JumpStateData _jumpStateData;
        public DashMovementState(Character ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _jumpStateData = stateData;
        }
        public override void Enter()
        {
            base.Enter();
            Direction direction = _character.GetRelativeMovementDirection();

            _character.EventBus.Publish(new JumpPerformedEvent
            {
                jumpType = JumpType.Dash,
                planarForce = _jumpStateData.PlanarJumpForce,
                direction = direction
            });
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Input.SprintPressing;
        }

    }
}
