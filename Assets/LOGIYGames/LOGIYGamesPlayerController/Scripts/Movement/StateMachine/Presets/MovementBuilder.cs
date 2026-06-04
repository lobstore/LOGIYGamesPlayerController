using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{

    public abstract class MovementBuilder : ScriptableObject
    {
        public abstract void Build(CharacterModule movementStateDriver);
    }
}
