using LOGIYGames.CharacterCore;
using R3;
using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public abstract class RuntimeEffect
    {
        protected Actor Owner;
        public EffectData Data;
        [SerializeField] protected bool isStackable;
        [SerializeField] protected bool isFinished;
        public SerializableReactiveProperty<string> DisplayValue = new();

        public bool IsFinished => isFinished;
        public bool IsStackable => isStackable;
        protected RuntimeEffect(EffectData effectData)
        {
            Data = effectData;
        }





        public virtual void Initialize(Actor owner)
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
