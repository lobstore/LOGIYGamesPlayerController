using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "RollState", menuName = "Character States/RollState")]
    public class RollState : MovementStateSO
    {
        public JumpStateData stateData;
        protected override CharacterMovementState CreateState(Character character)
        {
            return new RollMovementState(character, stateData);
        }
    }
}
