using Alchemy.Inspector;
using LOGIYGames.Timers;
using System;
using UnityEngine;

[Serializable]
public class DamageOverTimeEffect : DamageEffect
{
    [ReadOnly][field: SerializeField] public IntervalTimer Timer { get; private set; }
    public override bool IsFinished => Timer.IsFinished;
    public DamageOverTimeEffect(DamageEffectData effectData, float duration, float tickInterval) : base(effectData)
    {
        Timer = new IntervalTimer(duration, tickInterval, false);
        Timer.OnInterval += OnInterval;
    }

    public override void OnApply()
    {
        Timer.Start();
    }

    public override void OnRemove()
    {
        Timer.Stop();
    }

    public override void OnUpdate(float delta)
    {
        DisplayValue.Value = Mathf.RoundToInt(Timer.CurrentTime.CurrentValue).ToString();
        Timer.Tick();
    }

    private void OnInterval()
    {
        if (Data.VFX != null)
        {
            var obj = UnityEngine.Object.Instantiate(Data.VFX, Owner.transform.position + Vector3.up * 2f, Quaternion.identity);
            UnityEngine.Object.Destroy(obj, 1);
        }
        Owner.TakeDamage(DamageData.Damage);
    }

}