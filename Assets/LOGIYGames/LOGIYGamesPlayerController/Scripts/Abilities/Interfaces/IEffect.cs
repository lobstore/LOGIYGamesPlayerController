using System;
using UnityEngine;

namespace LOGIYGames
{
    public interface IEffect
    {
        void Apply(GameObject target);
        void Cancel();
    }
    public abstract class EffectFactory :ScriptableObject
    {
        public abstract IEffect CreateEffect();
    }
}
