using LOGIYGames.AI;
using UnityEngine;

namespace LOGIYGames
{

    public abstract class AIStatesBuilder : ScriptableObject
    {
        public abstract void Build(AIBrain AIBrainStateDriver);
    }
}
