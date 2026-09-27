using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "WallClimbStateFactory", menuName = "MovementStateMachine/Factories/WallClimbStateFactory")]
    public class WallClimbStateFactory : MovementStateFactory
    {
        public MovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new WallClimbMovementState(character, stateData);
        }
    }
}
