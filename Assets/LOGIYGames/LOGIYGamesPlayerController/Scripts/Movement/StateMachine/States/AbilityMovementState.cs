using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;

namespace LOGIYGames
{
    public class AbilityMovementState : MovementStateBase
    {
        //private readonly AbilityController abilityController;
        public AbilityMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            // abilityController = ctx.AbilityController;
        }

        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new NoneMovement();
            _character.RotationStrategy = new NoneRotation(_character);
            // abilityController.BeginAbility();
        }
        public override void Exit()
        {
            base.Exit();
        }
        public override bool CanExit()
        {
            return false;
            //abilityController.CurrentAbility == null;
        }
    }
}
