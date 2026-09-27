using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "TurnMovementStateFactory", menuName = "MovementStateMachine/Factories/TurnMovementStateFactory")]
    public class TurnMovementStateFactory : MovementStateFactory
    {
        public TurnMovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new TurnMovementState(character, stateData);
        }
    }
}
