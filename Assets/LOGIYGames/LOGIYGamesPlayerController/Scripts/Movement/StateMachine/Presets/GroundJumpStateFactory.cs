using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "GroundJumpState", menuName = "MovementStateMachine/States/GroundJumpState")]
    public class GroundJumpStateFactory : MovementStateFactory
    {
        public JumpStateData stateData;

        protected override CharacterMovementState CreateState(Character character)
        {
            return new GroundJumpMovementState(character, stateData);
        }
    }
}
