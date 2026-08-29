using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "RollStateFactory", menuName = "MovementStateMachine/Factories/RollStateFactory")]
    public class RollStateFactory : MovementStateFactory
    {
        public JumpStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new RollMovementState(character, stateData);
        }
    }
}
