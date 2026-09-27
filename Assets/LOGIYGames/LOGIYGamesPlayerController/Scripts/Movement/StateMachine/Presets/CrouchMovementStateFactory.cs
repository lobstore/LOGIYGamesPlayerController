using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "CrouchMovementStateFactory", menuName = "MovementStateMachine/Factories/CrouchMovementStateFactory")]
    public class CrouchMovementStateFactory : MovementStateFactory
    {
        public MovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new CrouchMovementState(character, stateData);
        }
    }
}
