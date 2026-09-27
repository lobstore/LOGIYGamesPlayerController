using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "WalkMovementStateFactory", menuName = "MovementStateMachine/Factories/WalkMovementStateFactory")]
    public class WalkMovementStateFactory : MovementStateFactory
    {
        public MovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new WalkMovementState(character, stateData);
        }
    }
}
