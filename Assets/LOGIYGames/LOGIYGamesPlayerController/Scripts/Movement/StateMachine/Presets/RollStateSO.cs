using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "RollState", menuName = "Character States/RollState")]
    public class RollStateSO : MovementStateSO
    {
        public JumpStateData stateData;
        protected override CharacterMovementState CreateState(CharacterModule character)
        {
            return new RollMovementState(character, stateData);
        }
    }
}
