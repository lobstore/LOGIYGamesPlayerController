using LOGIYGames.CharacterCore;
using UnityEngine.TextCore.Text;
namespace LOGIYGames.Movement
{
    public class SwimMovementState : MovementStateBase
    {
        public SwimMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
        public override bool CanEnter()
        {
            return _character.Sensors.IsInWater;
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
        public override bool CanExit()
        {
            return !_character.Sensors.IsInWater;
        }
    }

}
