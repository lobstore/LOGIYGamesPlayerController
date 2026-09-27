using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using UnityEngine;

namespace LOGIYGames
{
    public class BackTurnMovementState : TimedMovementState
    {
        LocomotionController controller;
        TurnMovementStateData TurnData;
        public BackTurnMovementState(Actor ctx, TurnMovementStateData stateData) : base(ctx, stateData)
        {
            TurnData = stateData;
            controller = ctx.GetComponent<LocomotionController>();
        }
        public override void Enter()
        {
            _character.MovementStrategy = new CharacterForwardMovement(_character);
            _character.EventBus.Publish(new BackTurnPerformedEvent
            {
                movementSpeed = _character.RuntimeMovement.CurrentSpeed,
                angle = _character.RuntimeMovement.DeltaYaw
            });
            base.Enter();
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (!Data.IsAnimationDrivenMovement)
            {
                controller.Move();

            }
            if (!Data.IsAnimationDrivenRotation)
            {
                _character.Rotate();

            }
        }
        public override bool CanEnter()
        {
            return base.CanEnter()
                && Mathf.Abs(_character.RuntimeMovement.DeltaYaw) > TurnData.MinAngle
                && _character.DefaultMovementStrategy is not StrafeMovement;
        }
    }

}
