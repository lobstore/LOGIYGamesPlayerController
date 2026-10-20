using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "WallrunMovementStateFactory", menuName = "MovementStateMachine/Factories/WallrunMovementStateFactory")]
    public class WallrunMovementStateFactory : MovementStateFactory
    {
        public MovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new WallRunMovementState(character, stateData);
        }
    }
}
