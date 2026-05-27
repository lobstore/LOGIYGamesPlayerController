using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public abstract class MovementStateSO : ScriptableObject
    {
        public void Build(CharacterModule character)
        {
            var state = CreateState(character);

            character.AddMovementState(state);
        }

        protected abstract CharacterMovementState CreateState(CharacterModule character);
    }
}
