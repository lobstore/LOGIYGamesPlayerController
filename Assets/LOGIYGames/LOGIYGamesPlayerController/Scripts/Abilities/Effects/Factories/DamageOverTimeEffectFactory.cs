using LOGIYGames;
using System;

[Serializable]
public class DamageOverTimeEffectFactory : EffectFactory
{
    public DamageOverTimeEffectData effectData;
    public override RuntimeEffect Create()
    {
        return new DamageOverTimeEffect(effectData);
    }
}
