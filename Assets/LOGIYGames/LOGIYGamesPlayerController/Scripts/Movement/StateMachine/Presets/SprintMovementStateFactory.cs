using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "SprintMovementStateFactory", menuName = "MovementStateMachine/Factories/SprintMovementStateFactory")]
    public class SprintMovementStateFactory : MovementStateFactory
    {
        public MovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new SprintMovementState(character, stateData);
        }
    }
}
