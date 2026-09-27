using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "LandingMovementStateFactory", menuName = "MovementStateMachine/Factories/LandingMovementStateFactory")]
    public class LandingMovementStateFactory : MovementStateFactory
    {
        public TimedMovementStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new LandingMovementState(character, stateData);
        }
    }
}
