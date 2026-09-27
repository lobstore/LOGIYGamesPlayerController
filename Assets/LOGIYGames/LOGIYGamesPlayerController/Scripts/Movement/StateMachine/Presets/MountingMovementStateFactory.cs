using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "MountingMovementStateFactory", menuName = "MovementStateMachine/Factories/MountingMovementStateFactory")]
    public class MountingMovementStateFactory : MovementStateFactory
    {
        public MovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new MountingMovementState(character,stateData);
        }
    }

}
