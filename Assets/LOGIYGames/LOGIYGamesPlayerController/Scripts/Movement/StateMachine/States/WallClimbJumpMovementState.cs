using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames.Movement
{
    public class WallClimbJumpMovementState : TimedMovementState
    {
        private JumpStateData _stateData;
        public WallClimbJumpMovementState(Character ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _stateData = stateData;

        }

        public override void Enter()
        {
            base.Enter();

            Direction direction = _character.GetRelativeMovementDirection();
            _character.EventBus.Publish(new JumpPerformedEvent
            {
                verticalForce = _stateData.VerticalJumpForce,
                planarForce = _stateData.PlanarJumpForce,
                direction = direction,
                jumpType = JumpType.HangJump

            });
        }
    }
}
