using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "SlipJumpMovementStateFactory", menuName = "MovementStateMachine/Factories/SlipJumpMovementStateFactory")]
    public class SlipJumpMovementStateFactory : MovementStateFactory
    {
        public JumpStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new SlipJumpMovementState(character, stateData);
        }
    }
}
