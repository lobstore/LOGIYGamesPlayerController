using LOGIYGames.CharacterCore;
namespace LOGIYGames.Movement
{
    public class SwimMovementState : CharacterMovementState
    {
        public SwimMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _character.RotationStrategy = new ToMovementDirectionRotation(_character);
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
