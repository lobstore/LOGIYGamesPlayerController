using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using UnityEngine;
namespace LOGIYGames.Movement
{
    public class WallRunMovementState : CharacterMovementState
    {
        Vector3 normal;
        public WallRunMovementState(CharacterModule ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
        public override void Enter()
        {
            base.Enter();
            _character.RotationStrategy = new ToMoveDirectionRotation(_character);
            _character.MovementStrategy = new WallRunMovement(_character);
            _controller.UseGravity = false;
            _character.IsWallRunning = true;
            _character.EventBus.Publish(new WallrunEnterEvent
            {
                IsRightSide = _character.Sensors.IsObstacleLegsRight ? true : false
            });
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            normal = _character.Sensors.IsObstacleLegsRight ? _character.Sensors.LegsRightHit.normal : _character.Sensors.LegsLeftHit.normal;
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _controller.ForceMove(-normal);
        }
        public override void Exit()
        {
            base.Exit();
            _character.IsWallRunning = false;
            _controller.UseGravity = true;
        }
    }
}
