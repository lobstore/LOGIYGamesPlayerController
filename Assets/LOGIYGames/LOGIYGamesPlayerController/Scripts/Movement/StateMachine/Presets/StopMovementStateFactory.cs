using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "StopMovementStateFactory", menuName = "MovementStateMachine/Factories/StopMovementStateFactory")]
    public class StopMovementStateFactory : MovementStateFactory
    {
        public TimedMovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new StopMovementState(character, stateData);
        }
    }
}
