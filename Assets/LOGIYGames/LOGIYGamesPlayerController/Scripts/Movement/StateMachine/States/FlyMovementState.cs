using LOGIYGames.CharacterCore;
namespace LOGIYGames.Movement
{
    public class FlyMovementState : MovementStateBase
    {
        public FlyMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new Input360LookMovement(_character);
            _character.GetComponent<MovementWrapperBase>().UseGravity = false;
        }
        public override void Exit()
        {
            base.Exit();
            _character.GetComponent<MovementWrapperBase>().UseGravity = true;
        }
    }
}
