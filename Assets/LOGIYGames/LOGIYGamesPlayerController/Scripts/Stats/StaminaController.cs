using LOGIYGames.Timers;
using R3;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class StaminaController
    {
        public Stamina Stamina { get; private set; }
        CountdownTimer _regenDelayTimer;
        Stat VITStat;
        public Stat MPStat { get; private set; }
        public Subject<float> StaminaUsed = new();
        public Subject<float> StaminaRestored = new();
        public Subject<Unit> Exhausted = new();
        public StaminaController(StatsController stats, float delayBeforeRegen = 0)
        {
            Stamina = new Stamina();
            MPStat = stats.GetStat(StatType.BaseStamina);
            VITStat = stats.GetStat(StatType.Vitality);
  
            _regenDelayTimer = new(delayBeforeRegen);
            StaminaUsed.Subscribe((_) =>
            {
                _regenDelayTimer.Start();
            });

            Stamina.Current = Stamina.Max;
            UpdateMaxValue();
        }

        private void UpdateMaxValue()
        {
            Stamina.Max = MPStat.Value + (VITStat.Value * MPStat.Value * 0.01f);
        }
        public void Tick()
        {
            var m_regenAmount = VITStat.Value / 10f + 1;
            if (CanRegenerate())
            {
                Restore(m_regenAmount * Time.deltaTime);

            }
            UpdateMaxValue();
        }
        private bool CanRegenerate()
        {
            return !_regenDelayTimer.IsRunning
                   && Stamina.Current < Stamina.Max;
        }
        public bool CanUse(float amount)
        {
            return Stamina.Current >= amount;
        }

        public bool TryUse(float amount)
        {
            if (!CanUse(amount))
            {
                Exhausted.OnNext(Unit.Default);
                return false;
            }

            Stamina.Current -= amount;

            StaminaUsed.OnNext(amount);

            return true;
        }

        public void Restore(float amount)
        {
            float previous = Stamina.Current;

            Stamina.Current = Mathf.Min(Stamina.Current + amount, Stamina.Max);

            float restored = Stamina.Current - previous;

            if (restored > 0)
            {
                StaminaRestored.OnNext(restored);
            }
        }
    }
}
