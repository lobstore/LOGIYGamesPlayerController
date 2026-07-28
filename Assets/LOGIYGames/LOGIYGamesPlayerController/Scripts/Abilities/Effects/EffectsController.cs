using LOGIYGames.CharacterCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inspector;
using R3;
namespace LOGIYGames
{
    [Serializable]
    public sealed class EffectsController
    {
        private readonly Character _owner;
        [ReadOnly][SerializeReference] private List<RuntimeEffect> _effects = new();
        public IReadOnlyList<RuntimeEffect> Effects => _effects;
        public Subject<IReadOnlyList<RuntimeEffect>> OnCollectionChanged = new();
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
            effect.Initialize(_owner);

            _effects.Add(effect);
            OnCollectionChanged.OnNext(_effects);
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
            OnCollectionChanged.OnNext(_effects);
        }
    }
}
