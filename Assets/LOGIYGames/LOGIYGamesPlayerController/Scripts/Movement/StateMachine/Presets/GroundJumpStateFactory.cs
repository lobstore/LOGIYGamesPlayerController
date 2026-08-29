using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "GroundJumpStateFactory", menuName = "MovementStateMachine/Factories/GroundJumpStateFactory")]
    public class GroundJumpStateFactory : MovementStateFactory
    {
        public JumpStateData stateData;

        protected override MovementStateBase CreateState(Actor character)
        {
            return new GroundJumpMovementState(character, stateData);
        }
    }

}
