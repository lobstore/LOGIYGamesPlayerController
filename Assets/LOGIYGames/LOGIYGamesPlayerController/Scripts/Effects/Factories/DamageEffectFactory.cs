using LOGIYGames;
using System;

[Serializable]
public class DamageEffectFactory : EffectFactory
{
    public DamageEffectData effectData;
    public override RuntimeEffect Create()
    {
        return new DamageEffect(effectData);
    }
}