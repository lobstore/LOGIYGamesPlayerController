using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "GroundJumpState", menuName = "Character States/GroundJumpState")]
    public class GroundJumpStateSO : MovementStateSO
    {
        public JumpStateData stateData;

        protected override CharacterMovementState CreateState(Character character)
        {
            return new GroundJumpMovementState(character, stateData);
        }
    }
}
