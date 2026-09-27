using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "RollMovementStateFactory", menuName = "MovementStateMachine/Factories/RollMovementStateFactory")]
    public class RollMovementStateFactory : MovementStateFactory
    {
        public JumpStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new RollMovementState(character, stateData);
        }
    }
}
