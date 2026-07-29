using LOGIYGames;
using System;
using UnityEngine;

[Serializable]
public class DamageEffect : InstantEffect
{
    [SerializeField] protected DamageData Damage;
    public DamageEffect(DamageEffectData effectData) : base(effectData)
    {
        Damage = effectData.BaseDamage;
    }
    public override void OnApply()
    {
        Owner.TakeDamage(Damage);
        IsFinished = true;
    }

    public override void OnRemove()
    {
    }

    public override void OnUpdate(float delta)
    {
    }

}
