using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "MantlingStateFactory", menuName = "MovementStateMachine/Factories/MantlingStateFactory")]
    public class MantlingStateFactory : MovementStateFactory
    {
        public MantlingMovmentStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new MantlingMovementState(character, stateData);
        }
    }
}
