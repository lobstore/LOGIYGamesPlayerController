using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public class WallClimbMovementState : MovementStateBase
    {
        SensorsModule sensorModule;
        ClimbController controller;
        public WallClimbMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            sensorModule = ctx.GetComponent<SensorsModule>();
            controller = ctx.GetComponent<ClimbController>();
        }
        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new WallClimbMovement(sensorModule, _character);
            _character.RotationStrategy = new WallClimbRotaion(sensorModule);
            _controller.UseGravity = false;
            _animator.SetBool("IsWallClimbing", true);
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (!Data.IsAnimationDrivenMovement)
            {
                controller.Climb();

            }
            if (!Data.IsAnimationDrivenRotation)
            {
                _character.Rotate();

            }
            controller.Magnit();
        }
        public override void Exit()
        {
            base.Exit();
            _animator.SetBool("IsWallClimbing",false);
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
                    _character.Input.MovementInput.magnitude > 0
                        && (_character.transform.InverseTransformDirection(_controller.Velocity).z > 0
                        || _character.transform.InverseTransformDirection(_controller.Velocity).y > 0);
        }
    }
}
