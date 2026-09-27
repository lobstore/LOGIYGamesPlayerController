using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "DashMovementStateFactory", menuName = "MovementStateMachine/Factories/DashMovementStateFactory")]
    public class DashStateFactory : MovementStateFactory
    {
        public JumpStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new DashMovementState(character, stateData);
        }
    }
}
