using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "RollState", menuName = "MovementStateMachine/States/RollState")]
    public class RollStateFactory : MovementStateFactory
    {
        public JumpStateData stateData;
        protected override CharacterMovementState CreateState(CharacterModule character)
        {
            return new RollMovementState(character, stateData);
        }
    }
}
