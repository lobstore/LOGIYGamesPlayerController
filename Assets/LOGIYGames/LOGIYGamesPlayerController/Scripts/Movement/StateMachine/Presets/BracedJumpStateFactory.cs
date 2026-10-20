using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "BracedJumpStateFactory", menuName = "MovementStateMachine/Factories/BracedJumpStateFactory")]
    public class BracedJumpStateFactory : MovementStateFactory
    {
        public JumpStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new BracedJumpMovementState(character, stateData);
        }
    }
}
