using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "SwimMovementStateFactory", menuName = "MovementStateMachine/Factories/SwimMovementStateFactory")]
    public class SwimMovementStateFactory : MovementStateFactory
    {
        public MovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new SwimMovementState(character, stateData);
        }
    }
}
