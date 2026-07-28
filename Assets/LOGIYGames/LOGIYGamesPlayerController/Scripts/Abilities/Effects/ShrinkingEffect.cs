using Alchemy.Inspector;
using LOGIYGames;
using LOGIYGames.Timers;
using System;
using UnityEngine;

[Serializable]
public class ShrinkingEffect : RuntimeEffect
{
    [ReadOnly][field: SerializeField] public CountdownTimer Timer { get; private set; }
    public override bool IsFinished => Timer.IsFinished;
    public ShrinkingEffect(EffectData effectData, float duration) : base(effectData)
    {
        Timer = new CountdownTimer(duration, false);
    }

    public override void OnApply()
    {
        Timer.Start();
        Owner.gameObject.transform.localScale = Vector3.one * 0.1f;
    }

    public override void OnRemove()
    {
        Timer.Stop();
        Owner.gameObject.transform.localScale = Vector3.one;
    }

    public override void OnUpdate(float delta)
    {
        DisplayValue.Value = Mathf.RoundToInt(Timer.CurrentTime.CurrentValue).ToString();
        Timer.Tick();
    }

}