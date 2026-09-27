using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "FlyMovementStateFactory", menuName = "MovementStateMachine/Factories/FlyMovementStateFactory")]
    public class FlyMovementStateFactory : MovementStateFactory
    {
        public MovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new FlyMovementState(character, stateData);
        }
    }
}
