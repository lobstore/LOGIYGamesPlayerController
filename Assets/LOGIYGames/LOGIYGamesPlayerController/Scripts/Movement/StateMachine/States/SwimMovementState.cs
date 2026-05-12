using LOGIYGames.CharacterCore;
namespace LOGIYGames.Movement
{
    public class SwimMovementState : BaseCharacterMovementState
    {
        public SwimMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _character.RotationStrategy = new SwimRotation(_character);
            _character.MovementStrategy = new SwimMovement(_character);
            _character. IsSwimming = true;
            _character.GetComponent<ControllerWrapperBase>().UseGravity = false;
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            //if (_character.Sensors.AboveHit.collider == null)
            //{
            //    return;
            //}
            //if (_character.Sensors.AboveHit.collider.CompareTag("Water") && _character.Input.MovementInput.magnitude==0)
            //{
            //    _character.GetComponent<CharacterGravityModule>().GravityDirection = Vector3.up;
            //    _character.GetComponent<CharacterGravityModule>().MaxGravityForce = 1;

            //}
            //else
            //{
            //    _character.GetComponent<CharacterGravityModule>().GravityDirection = Vector3.down;
            //    _character.GetComponent<CharacterGravityModule>().MaxGravityForce = 15;
            //}

        }
        public override void Exit()
        {
            base.Exit();
            _character.RotationStrategy = _character.DefaultRotationStrategy;
            _character.MovementStrategy = _character.DefaultMovementStrategy;
            _character.GetComponent<ControllerWrapperBase>().UseGravity = true;
            _character.IsSwimming = false;
        }
    }

}
