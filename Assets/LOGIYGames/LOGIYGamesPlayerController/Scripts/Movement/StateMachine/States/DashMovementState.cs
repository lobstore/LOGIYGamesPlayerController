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
        public DashMovementState(Actor ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _jumpStateData = stateData;
        }
        public override void Enter()
        {
            base.Enter();

            if (!Data.IsAnimationDrivenMovement)
            {
                _character.EventBus.Publish(new JumpPerformedEvent
                {

                    jumpType = JumpType.Dash,
                    planarForce = _jumpStateData.PlanarJumpForce,
                    verticalForce = _jumpStateData.VerticalJumpForce,
                    direction = _character.GetInputRelativeDirection()
                });
            }
            else
            {
                _character.EventBus.Publish(new JumpPerformedEvent
                {

                    jumpType = JumpType.Dash,
                    planarForce = 0,
                    verticalForce = 0,
                    direction = _character.GetInputRelativeDirection()
                });
            }

        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Input.SprintPressing;
        }

    }
}
