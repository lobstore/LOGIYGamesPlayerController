using LOGIYGames.AI;
using UnityEngine;

namespace LOGIYGames
{

    public abstract class AIStatesPresetBase : ScriptableObject
    {
        public abstract void Init(AIBrainStateDriver AIBrainStateDriver);
    }
}
