using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "WallrunJumpStateFactory", menuName = "MovementStateMachine/Factories/WallrunJumpStateFactory")]
    public class WallrunJumpStateFactory : MovementStateFactory
    {
        public JumpStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new WallrunJumpMovementState(character, stateData);
        }
    }
}
