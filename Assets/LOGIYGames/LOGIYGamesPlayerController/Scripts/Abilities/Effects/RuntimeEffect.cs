using Alchemy.Inspector;
using LOGIYGames.CharacterCore;
using R3;
using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public abstract class RuntimeEffect
    {
        protected Character Owner;
        [ReadOnly][field: SerializeField] public EffectData Data { get; protected set; }
        protected RuntimeEffect(EffectData effectData)
        {
            Data = effectData;
        }
        protected bool isStackable;
        public bool IsStackable => isStackable;

        [ReadOnly][field: SerializeField] public virtual bool IsFinished { get; protected set; }

        public ReactiveProperty<string> DisplayValue { get; protected set; } = new();



        public virtual void Initialize(Character owner)
        {
            Owner = owner;
        }


        public abstract void OnApply();


        public virtual void OnUpdate(float delta)
        {
        }


        public virtual void OnRemove()
        {
        }
    }
    [Serializable]
    public abstract class InstantEffect : RuntimeEffect
    {
        public InstantEffect(EffectData effectData) : base(effectData)
        {
        }

    }
    [Serializable]
    public abstract class ContinuousEffect : RuntimeEffect
    {
        public ContinuousEffect(EffectData effectData) : base(effectData)
        {
        }

    }
}
