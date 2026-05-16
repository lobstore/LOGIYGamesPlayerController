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
            _character.RotationStrategy = new SwimRotation(_character);
            _character.MovementStrategy = new SwimMovement(_character);
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
