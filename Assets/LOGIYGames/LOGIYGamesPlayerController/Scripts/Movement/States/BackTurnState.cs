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
            _character.OnBackTurnStart.Invoke();
            _character.CurrentRotationStrategy = new NoneRotation(_character.transform);
        }

        public override void Exit()
        {
            base.Exit();
            _character.OnBackTurnEnd.Invoke();
            _character.CurrentRotationStrategy = _character.DefaultRotaionStrategy;
        }
    }
}
