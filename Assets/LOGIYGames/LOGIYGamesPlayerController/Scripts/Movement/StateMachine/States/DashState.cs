using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames
{
    public class DashState : TimedMovementState
    {
        private JumpStateData _jumpStateData;
        public DashState(Character ctx, JumpStateData stateData) : base(ctx, stateData)
        {
            _jumpStateData = stateData;
        }
        public override void Enter()
        {
            base.Enter();
            Vector3 localDir = _character.transform.InverseTransformDirection(_character.targetDirection);
            float forwardDot = Vector3.Dot(localDir, Vector3.forward);
            float rightDot = Vector3.Dot(localDir, Vector3.right);
            Direction direction;
            // Сравниваем проекции, чтобы определить направление
            if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
            {
                if (forwardDot > 0)
                    direction = Direction.Forward;
                else
                    direction = Direction.Backward;
            }
            else
            {
                if (rightDot > 0)
                    direction = Direction.Right;
                else
                    direction = Direction.Left;
            }

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
