using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public abstract class TargetingStrategy
    {
        public abstract void Start(AbilityContext context);
        public abstract void Update();
        public abstract void Cancel();
    }
}
