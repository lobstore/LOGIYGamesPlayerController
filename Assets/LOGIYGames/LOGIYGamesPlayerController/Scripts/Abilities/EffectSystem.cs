using LOGIYGames.CharacterCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public sealed class EffectSystem
    {
        private Character _owner;

        [SerializeField] private List<RuntimeEffect> _effects = new();
        public List<RuntimeEffect> Effects => _effects;

        public EffectSystem(Character owner)
        {
            _owner = owner;
        }
        public void Update()
        {
            if (_effects.Count > 0)
            {
                foreach (var item in _effects)
                {
                    item.OnUpdate(Time.deltaTime);
                }
            }
        }
        public void AddEffect(RuntimeEffect effect)
        {
            effect.Timer.OnTimerStop += () =>
            {
                RemoveEffect(effect);
            };
            effect.Owner = _owner;

            _effects.Add(effect);

            effect.OnApply();
        }
        public void RemoveEffect(RuntimeEffect effect)
        {
            if (_effects.Contains(effect))
            {
                _effects.Remove(effect);
                effect.OnRemove();
            }
        }
    }

}
