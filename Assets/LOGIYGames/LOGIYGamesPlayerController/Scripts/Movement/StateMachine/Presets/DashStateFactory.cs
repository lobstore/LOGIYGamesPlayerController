using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "DashState", menuName = "MovementStateMachine/States/DashState")]
    public class DashStateFactory : MovementStateFactory
    {
        public JumpStateData stateData;
        protected override CharacterMovementState CreateState(CharacterModule character)
        {
            return new DashMovementState(character, stateData);
        }
    }
}
