using LOGIYGames.CharacterCore;
namespace LOGIYGames.Movement
{
    public class SwimMovementState : CharacterMovementState
    {
        public SwimMovementState(CharacterModule ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _character.RotationStrategy = new ToMovementDirectionRotation(_character);
            _character.MovementStrategy = new Input360LookMovement(_character);
            _character.IsSwimming = true;
            _character.GetComponent<ControllerWrapperBase>().UseGravity = false;
        }
        public override void Exit()
        {
            base.Exit();
            _character.GetComponent<ControllerWrapperBase>().UseGravity = true;
            _character.IsSwimming = false;
        }
    }

}
