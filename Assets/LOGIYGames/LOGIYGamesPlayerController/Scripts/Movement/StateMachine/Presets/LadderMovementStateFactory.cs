using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "LadderMovementStateFactory", menuName = "MovementStateMachine/Factories/LadderMovementStateFactory")]
    public class LadderMovementStateFactory : MovementStateFactory
    {
        public MovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new LadderMovementState(character, stateData);
        }
    }
}
