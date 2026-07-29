using Alchemy.Inspector;
using LOGIYGames;
using LOGIYGames.Audio;
using LOGIYGames.Timers;
using System;
using UnityEngine;

[Serializable]
public class DamageOverTimeEffect : ContinuousEffect
{
    [SerializeField] protected DamageData Damage;
    [ReadOnly][field: SerializeField] public IntervalTimer Timer { get; private set; }
    public override bool IsFinished => Timer.IsFinished;
    [ReadOnly][SerializeField] float duration;
    [ReadOnly][SerializeField] float interval;
    private GameObject effectGO;
    public DamageOverTimeEffect(DamageOverTimeEffectData effectData) : base(effectData)
    {
        isStackable = true;
        Damage = effectData.BaseDamage;
        duration = effectData.Duration;
        interval = effectData.Interval;
        Timer = new IntervalTimer(effectData.Duration, effectData.Interval, false);
        Timer.OnInterval += OnInterval;
    }

    public override void OnApply()
    {
        Timer.Start();
        if (Data.VFX != null)
        {
            effectGO = UnityEngine.Object.Instantiate(Data.VFX, Owner.transform.position, Quaternion.identity, Owner.transform);

        }
    }

    public override void OnRemove()
    {
        Timer.Stop();
        if (effectGO != null)
        UnityEngine.Object.Destroy(effectGO);
    }

    public override void OnUpdate(float delta)
    {
        DisplayValue.Value = Mathf.RoundToInt(Timer.CurrentTime.CurrentValue).ToString();
        Timer.Tick();
    }

    private void OnInterval()
    {

        Owner.TakeDamage(Damage);
        SoundManager.Instance.CreateSoundBuilder().WithPosition(Owner.transform.position).WithRandomPitch(-0.1f, 0.1f).Play(new SoundData() { clip = Data.SFX } );
    }

}