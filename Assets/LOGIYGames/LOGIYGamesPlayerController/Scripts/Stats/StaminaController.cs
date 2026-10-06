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
            MPStat.OnModifiersChanged.Subscribe((_) =>
            {
                UpdateMaxValue();
            });
            Stamina.Max.Subscribe((value) =>
            {
                Stamina.Current.Value = Mathf.Min(Stamina.Current.CurrentValue, Stamina.Max.CurrentValue);
            });
            Stamina.Current.Value = Stamina.Max.CurrentValue;
            UpdateMaxValue();
        }

        private void UpdateMaxValue()
        {
            Stamina.Max.Value = MPStat.Value + (VITStat.Value * MPStat.Value * 0.01f);
        }
        public void Tick()
        {
            var m_regenAmount = VITStat.Value / 10f + 1;
            if (CanRegenerate())
            {
                Restore(m_regenAmount * Time.deltaTime);

            }
        }
        private bool CanRegenerate()
        {
            return !_regenDelayTimer.IsRunning
                   && Stamina.Current.CurrentValue < Stamina.Max.CurrentValue;
        }
        public bool CanUse(float amount)
        {
            return Stamina.Current.CurrentValue >= amount;
        }

        public bool TryUse(float amount)
        {
            if (!CanUse(amount))
            {
                Exhausted.OnNext(Unit.Default);
                return false;
            }

            Stamina.Current.Value -= amount;

            StaminaUsed.OnNext(amount);

            return true;
        }

        public void Restore(float amount)
        {
            float previous = Stamina.Current.Value;

            Stamina.Current.Value = Mathf.Min(Stamina.Current.CurrentValue + amount, Stamina.Max.CurrentValue);

            float restored = Stamina.Current.CurrentValue - previous;

            if (restored > 0)
            {
                StaminaRestored.OnNext(restored);
            }
        }
    }
}
