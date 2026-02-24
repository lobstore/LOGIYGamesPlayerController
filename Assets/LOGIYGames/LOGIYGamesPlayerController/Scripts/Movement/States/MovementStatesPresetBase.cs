using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{

    public abstract class MovementStatesPresetBase : ScriptableObject
    {
        public abstract void Init(MovementStateDriver movementStateDriver);
    }
}
