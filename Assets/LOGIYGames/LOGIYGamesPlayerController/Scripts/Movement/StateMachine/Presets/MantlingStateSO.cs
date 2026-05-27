using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "MantlingState", menuName = "Character States/MantlingState")]
    public class MantlingStateSO : MovementStateSO
    {
        public MantlingMovmentStateData stateData;
        protected override CharacterMovementState CreateState(CharacterModule character)
        {
            return new MantlingMovementState(character, stateData);
        }
    }
}
