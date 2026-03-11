using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public class BackTurnState : TimedState
    {
        public BackTurnState(MovementStateDriver ctx, TimedMovementStateData stateData) : base(ctx, stateData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _character.CurrentRotationStrategy = new NoneRotation(_character.transform);
            _character.CurrentMovementStrategy = new NoneMovement();
            _character.TurnBack();
        }

        public override void Exit()
        {
            base.Exit();
            _character.CurrentRotationStrategy = _character.DefaultRotaionStrategy;
            _character.CurrentMovementStrategy = _character.DefaultMovementStrategy;
        }
    }
}
