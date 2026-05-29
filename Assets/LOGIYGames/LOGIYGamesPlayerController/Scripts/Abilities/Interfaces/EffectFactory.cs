using UnityEngine;

namespace LOGIYGames
{
    public abstract class EffectFactory :ScriptableObject
    {
        public abstract IEffect CreateEffect();
    }
}
