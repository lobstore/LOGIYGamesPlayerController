using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "DashState", menuName = "Character States/DashState")]
    public class DashStateSO : MovementStateSO
    {
        public JumpStateData stateData;
        protected override CharacterMovementState CreateState(CharacterModule character)
        {
            return new DashMovementState(character, stateData);
        }
    }
}
