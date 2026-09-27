using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "BackTurnMovementStateFactory", menuName = "MovementStateMachine/Factories/BackTurnMovementStateFactory")]
    public class BackTurnMovementStateFactory : MovementStateFactory
    {
        public TurnMovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new BackTurnMovementState(character, stateData);
        }
    }
}
