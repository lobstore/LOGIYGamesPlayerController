using Alchemy.Inspector;
using LOGIYGames;
using System;
using UnityEngine;

[Serializable]
public class DamageEffect : RuntimeEffect
{
    [ReadOnly][field:SerializeField] public DamageEffectData DamageData { get; protected set; }
    public DamageEffect(DamageEffectData effectData) : base(effectData)
    {
        DamageData = effectData;
    }
    public override void OnApply()
    {
        Owner.TakeDamage(DamageData.Damage);
        IsFinished = true;
    }

    public override void OnRemove()
    {
    }

    public override void OnUpdate(float delta)
    {
    }

}
