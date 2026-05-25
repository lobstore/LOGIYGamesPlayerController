using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "DashState", menuName = "Character States/DashState")]
    public class DashState : MovementStateSO
    {
        public JumpStateData stateData;
        protected override CharacterMovementState CreateState(Character character)
        {
            return new DashMovementState(character, stateData);
        }
    }
}
