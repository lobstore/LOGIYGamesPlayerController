using LOGIYGames;
using System;
using UnityEngine;

[Serializable]
public class ShrinkingEffectFactory : EffectFactory
{
    public float duration = 3f;
    public EffectData effectData;
    public override RuntimeEffect Create()
    {
        return new ShrinkingEffect(effectData, duration);
    }
}