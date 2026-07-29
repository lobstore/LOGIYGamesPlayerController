using LOGIYGames.CharacterCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inspector;
using R3;
using System.Linq;
namespace LOGIYGames
{
    [Serializable]
    public sealed class EffectsController
    {
        private readonly Character _owner;
        [ReadOnly][SerializeReference] private List<RuntimeEffect> _effects = new();
        public IReadOnlyList<RuntimeEffect> Effects => _effects;
        public Subject<IReadOnlyList<RuntimeEffect>> OnContinuousEffectsChanged = new();
        public EffectsController(Character owner)
        {
            _owner = owner;
        }


        public void Update()
        {
            float delta = Time.deltaTime;


            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                RuntimeEffect effect = _effects[i];

                effect.OnUpdate(delta);


                if (effect.IsFinished)
                {
                    RemoveEffect(effect);
                }
            }
        }


        public void AddEffect(RuntimeEffect effect)
        {
            var existingEffect = _effects.FirstOrDefault(x => x.GetType() == effect.GetType());

            if (existingEffect != null && !effect.IsStackable)
            {
                RemoveEffect(existingEffect);
            }
            effect.Initialize(_owner);

            _effects.Add(effect);
            if (effect is ContinuousEffect)
            OnContinuousEffectsChanged.OnNext(_effects);
            effect.OnApply();


            // Для мгновенных эффектов
            if (effect.IsFinished)
            {
                RemoveEffect(effect);
            }
        }


        public void RemoveEffect(RuntimeEffect effect)
        {
            if (_effects.Remove(effect))
            {
                effect.OnRemove();
            }
            if (effect is ContinuousEffect)
                OnContinuousEffectsChanged.OnNext(_effects);
        }
    }
}
