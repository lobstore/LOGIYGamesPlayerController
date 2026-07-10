using R3;
using System;
using UnityEngine;
using UnityEngine.Events;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class Health
    {
        public ReadOnlyReactiveProperty<float> Current => _current;
        private SerializableReactiveProperty<float> _current = new();
        public ReadOnlyReactiveProperty<float> Max => _max;
        private SerializableReactiveProperty<float> _max = new();
        public Stat HPStat { get; private set; }
        public Stat VitStat { get; private set; }

        public readonly UnityEvent Died = new();

        public Health(CharacterStats stats)
        {
            HPStat = stats.GetStat(StatType.BaseHealth);
            VitStat = stats.GetStat(StatType.Vitality);
            HPStat.OnModifiersChanged.Subscribe((_) =>
            {
                UpdateMaxValue();
            });
            VitStat.OnModifiersChanged.Subscribe((_) =>
            {
                UpdateMaxValue();
            });
            _max.Subscribe((value) =>
            {
                _current.Value = Math.Min(_current.CurrentValue, _max.CurrentValue);
            });
            UpdateMaxValue();
            _current.Value = _max.CurrentValue;
        }

        private void UpdateMaxValue()
        {
            _max.Value = HPStat.Value + VitStat.Value * HPStat.Value;
        }

        public void ApplyDamage(float value)
        {
            if (_current.Value <= 0)
                return;

            _current.Value = Mathf.Max(0, _current.Value - value);

            if (_current.Value == 0)
                Died.Invoke();
        }

        public void ApplyHeal(float value)
        {
            _current.Value = Mathf.Min(_current.Value + value, _max.CurrentValue);
        }
    }
}
