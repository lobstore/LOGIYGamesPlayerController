using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine.TextCore.Text;

namespace LOGIYGames
{
    public class WallClimbMovementState : MovementStateBase
    {
        SensorsModule sensorModule;
        public WallClimbMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            sensorModule = ctx.GetComponent<SensorsModule>();
        }
        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new WallClimbMovement(sensorModule, _character);
            _character.RotationStrategy = new WallClimbRotaion(sensorModule);
            _controller.UseGravity = false;
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _controller.ForceMove(-sensorModule.LegsFrontHit.normal * 0.1f);
        }
        public override void Exit()
        {
            base.Exit();
            _character.MovementStrategy = _character.DefaultMovementStrategy;
            _character.RotationStrategy = _character.DefaultRotationStrategy;
            _controller.UseGravity = true;
        }
        public override bool CanExit()
        {
            return _character.Input.InteractPressed ||
                    !_character.Sensors.IsObstacleLegsFront ||
                    !_character.Sensors.LegsFrontHit.collider.CompareTag("Climbable") ||
                    (_character.IsGrounded && _character.Input.MovementInput.y < 0);
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Sensors.IsObstacleLegsFront &&
                    _character.Sensors.LegsFrontHit.collider.CompareTag("Climbable") &&
                    _character.Input.MovementInput.y > 0;
        }
    }
}
