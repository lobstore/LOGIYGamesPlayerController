using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public abstract class MovementStateFactory : ScriptableObject
    {
        public void Create(Actor character)
        {
            var state = CreateState(character);

            character.AddMovementState(state);
        }

        protected abstract MovementStateBase CreateState(Actor character);
    }
}
