using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using UnityEngine;

namespace LOGIYGames
{
    public class AbilityMovementState : CharacterMovementState
    {
        private readonly AbilityController abilityController;
        public AbilityMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
           // abilityController = ctx.AbilityController;
        }

        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new NoneMovement();
            _character.RotationStrategy = new NoneRotation(_character);
            abilityController.BeginAbility();
        }
        public override void Exit()
        {
            base.Exit();
        }
        public bool CanExit()
        {
            return abilityController.CurrentAbility == null;
        }
    }
}
