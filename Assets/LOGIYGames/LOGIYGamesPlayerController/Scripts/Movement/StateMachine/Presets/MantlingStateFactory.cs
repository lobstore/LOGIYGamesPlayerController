using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "MantlingState", menuName = "MovementStateMachine/States/MantlingState")]
    public class MantlingStateFactory : MovementStateFactory
    {
        public MantlingMovmentStateData stateData;
        protected override CharacterMovementState CreateState(Character character)
        {
            return new MantlingMovementState(character, stateData);
        }
    }
}
