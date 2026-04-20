using LOGIYGames.CharacterCore;
namespace LOGIYGames.Movement
{
    public class FlyMovementState : BaseMovementState
    {
        public FlyMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new FlyMovement(_character);
            _character.GetComponent<ControllerWrapperBase>().UseGravity = false;
            _character.IsFlying = true;
        }
        public override void Exit()
        {
            base.Exit();
            _character.MovementStrategy = _character.DefaultMovementStrategy;
            _character.GetComponent<ControllerWrapperBase>().UseGravity = true;
            _character.IsFlying = false;
        }
    }
}
