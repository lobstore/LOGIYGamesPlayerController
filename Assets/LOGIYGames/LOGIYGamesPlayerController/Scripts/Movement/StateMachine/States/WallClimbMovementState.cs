using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;

namespace LOGIYGames
{
    public class WallClimbMovementState : MovementStateBase
    {
        ClimbController controller;
        public WallClimbMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            controller = ctx.GetComponent<ClimbController>();
        }
        public override void Enter()
        {
            base.Enter();
            controller.Enter();
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            if (!Data.IsAnimationDrivenMovement)
            {
                controller.Climb();

            }
            controller.Magnit();
        }
        public override void Exit()
        {
            base.Exit();
            controller.Exit();
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
