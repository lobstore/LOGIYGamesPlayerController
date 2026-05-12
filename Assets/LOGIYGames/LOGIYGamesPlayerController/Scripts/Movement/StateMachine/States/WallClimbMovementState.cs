using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public class WallClimbMovementState : BaseCharacterMovementState
    {
        SensorsModule sensorModule;
        public WallClimbMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            sensorModule = ctx.GetComponent<SensorsModule>();
        }
        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new WallClimbMovement(sensorModule, _character);
            _character.RotationStrategy = new WallClimbRotaion(sensorModule);
            _controller.UseGravity = false;
            _character.IsWallClimbing = true;
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _character.ForceMove(-sensorModule.LegsFrontHit.normal*0.1f);
        }
        public override void Exit()
        {
            base.Exit();
            _character.MovementStrategy = _character.DefaultMovementStrategy;
            _character.RotationStrategy = _character.DefaultRotationStrategy;
            _controller.UseGravity = true;
            _character.IsWallClimbing = false;
        }
    }
}
