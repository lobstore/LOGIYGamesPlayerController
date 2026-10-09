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
        public HealthController(StatsController stats)
        {
            Health = new();
            VITStat = stats.GetStat(StatType.Vitality);
            HPStat = stats.GetStat(StatType.BaseHealth);
            UpdateMaxValue();
            Health.Current = Health.Max;
        }
        public void Tick()
        {
            UpdateMaxValue();
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
                    resultDamage = Health.Max * damage.Amount;
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
            if (Health.Current <= 0)
                return;

            Health.Current = Mathf.Max(0, Health.Current - value);

            if (Health.Current == 0)
                Died.Invoke();
        }
        private void IncreaseHealth(float value)
        {
            Health.Current = Mathf.Min(Health.Current + value, Health.Max);
        }
        private void UpdateMaxValue()
        {
            Health.Max = HPStat.Value + VITStat.Value * HPStat.Value;
        }
    }
}
