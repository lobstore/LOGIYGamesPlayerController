using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "ComboMovementStateFactory", menuName = "MovementStateMachine/Factories/ComboMovementStateFactory")]
    public class ComboMovementStateFactory : MovementStateFactory
    {
        public MovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new ComboMovementState(character, stateData);
        }
    }
}
