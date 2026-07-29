using LOGIYGames;
using System;

[Serializable]
public class ShrinkingEffectFactory : EffectFactory
{
    public ShrinkingEffectData effectData;
    public override RuntimeEffect Create()
    {
        return new ShrinkingEffect(effectData);
    }
}
