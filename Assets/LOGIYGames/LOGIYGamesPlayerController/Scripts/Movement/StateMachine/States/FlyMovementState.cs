using LOGIYGames.CharacterCore;
namespace LOGIYGames.Movement
{
    public class FlyMovementState : CharacterMovementState
    {
        public FlyMovementState(CharacterModule ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new Input360LookMovement(_character);
            _character.GetComponent<MovementWrapperBase>().UseGravity = false;
            _character.IsFlying = true;
        }
        public override void Exit()
        {
            base.Exit();
            _character.GetComponent<MovementWrapperBase>().UseGravity = true;
            _character.IsFlying = false;
        }
    }
}
