using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "HangJumpMovementStateFactory", menuName = "MovementStateMachine/Factories/HangJumpMovementStateFactory")]
    public class HangJumpMovementStateFactory : MovementStateFactory
    {
        public JumpStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new HangJumpMovementState(character, stateData);
        }
    }
}
