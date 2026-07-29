using R3;
using System;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

namespace LOGIYGames.CharacterCore
{
    public class HealthController
    {
        public Health Health { get; private set; }
        Stat VITStat;
        Stat HPStat;
        public readonly UnityEvent Died = new();
        public HealthController(CharacterStats stats)
        {
            Health = new();
            VITStat = stats.GetStat(StatType.Vitality);
            HPStat = stats.GetStat(StatType.BaseHealth);
            HPStat.OnModifiersChanged.Subscribe((_) =>
            {
                UpdateMaxValue();
            });
            VITStat.OnModifiersChanged.Subscribe((_) =>
            {
                UpdateMaxValue();
            });
            Health.Max.Subscribe((value) =>
            {
                Health.Current.Value = Math.Min(Health.Current.CurrentValue, Health.Max.CurrentValue);
            });
            UpdateMaxValue();
            Health.Current.Value = Health.Max.CurrentValue;
        }

        public void TakeDamage(DamageData damage)
        {

            float resultDamage = 0;
            switch (damage.ModifierType)
            {
                case ModifierType.Add:
                    resultDamage = Math.Clamp(damage.Amount - VITStat.Value, 0, float.MaxValue);
                    break;
                case ModifierType.Multiply:
                    resultDamage = Health.Max.CurrentValue * damage.Amount;
                    break;
                default:
                    break;
            }
            ReduceHealth(resultDamage);
        }
        public void TakeHeal(float amount)
        {
            IncreaseHealth(amount);
        }
        private void ReduceHealth(float value)
        {
            Debug.Log("DamageTook: " + value);
            if (Health.Current.Value <= 0)
                return;

            Health.Current.Value = Mathf.Max(0, Health.Current.Value - value);

            if (Health.Current.Value == 0)
                Died.Invoke();
        }
        private void IncreaseHealth(float value)
        {
            Health.Current.Value = Mathf.Min(Health.Current.Value + value, Health.Max.CurrentValue);
        }
        private void UpdateMaxValue()
        {
            Health.Max.Value = HPStat.Value + VITStat.Value * HPStat.Value;
        }
    }
}
