using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "DashStateFactory", menuName = "MovementStateMachine/Factories/DashStateFactory")]
    public class DashStateFactory : MovementStateFactory
    {
        public JumpStateData stateData;
        protected override MovementStateBase CreateState(Actor character)
        {
            return new DashMovementState(character, stateData);
        }
    }
}
