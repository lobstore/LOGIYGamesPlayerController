using LOGIYGames.Shared.Character.Events;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public abstract class TargetingFactory : ScriptableObject
    {
        public List<EffectFactory> Effects;
        public AbilityVFXData vFXData;
        public abstract TargetingStrategy Create();
    }
}
