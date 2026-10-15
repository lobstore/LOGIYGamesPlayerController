using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class FallingMovementState : MovementStateBase
    {
        FallingController controller;
        public FallingMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            controller = ctx.GetComponent<FallingController>();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            controller.Glide();
            _character.Rotate();
        }
    }

}
