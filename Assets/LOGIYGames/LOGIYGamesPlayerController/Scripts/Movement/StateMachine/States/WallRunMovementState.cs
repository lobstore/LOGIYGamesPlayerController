using LOGIYGames.CharacterCore;
namespace LOGIYGames.Movement
{
    public class WallRunMovementState : MovementStateBase
    {
        WallrunController controller;
        public WallRunMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            controller = ctx.GetComponent<WallrunController>();
        }
        public override void Enter()
        {
            base.Enter();
            controller.Enter();
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            controller.LogicUpdate();
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            controller.Rotate();
        }
        public override void Exit()
        {
            base.Exit();
            controller.Exit();
        }
        public override bool CanEnter()
        {
            return base.CanEnter() && controller.CanEnter();
        }
    }
}
