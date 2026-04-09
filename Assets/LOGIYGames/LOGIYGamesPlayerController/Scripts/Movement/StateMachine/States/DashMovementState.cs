using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;

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

            _character.EventBus.Publish(new DashPerformedEvent
            {
                planarForce = _jumpStateData.PlanarJumpForce,
                direction = direction
            });
        }

        protected override void Rotate()
        {

        }

    }
}
