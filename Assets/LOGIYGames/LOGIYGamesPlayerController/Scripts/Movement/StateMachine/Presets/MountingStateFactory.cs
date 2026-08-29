using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "MountingStateFactory", menuName = "MovementStateMachine/Factories/MountingStateFactory")]
    public class MountingStateFactory : MovementStateFactory
    {
        public MovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new MountingMovementState(character,stateData);
        }
    }

}
