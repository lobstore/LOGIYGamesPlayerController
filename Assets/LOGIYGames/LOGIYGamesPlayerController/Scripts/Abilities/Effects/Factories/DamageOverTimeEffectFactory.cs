using LOGIYGames;
using System;

[Serializable]
public class DamageOverTimeEffectFactory : EffectFactory
{
    public float duration = 3f;
    public float tickInterval = 1f;
    public DamageEffectData effectData;
    public override RuntimeEffect Create()
    {
        return new DamageOverTimeEffect(effectData, duration, tickInterval);
    }
}
