using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;

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
            _animator.SetBool("IsWallClimbing", false);
            _controller.UseGravity = true;
        }
        public override bool CanExit()
        {
            return base.CanExit() && controller.CanExit();
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && controller.CanEnter();
        }
    }
}
