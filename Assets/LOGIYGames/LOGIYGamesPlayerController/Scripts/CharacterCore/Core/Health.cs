using R3;
using UnityEngine;
using UnityEngine.Events;
namespace LOGIYGames.CharacterCore
{
    public class Health
    {
        public ReadOnlyReactiveProperty<float> Current => _current;
        public ReadOnlyReactiveProperty<float> Max => _max;

        private readonly ReactiveProperty<float> _current = new();
        private readonly ReactiveProperty<float> _max = new();

        public readonly UnityEvent Died = new();

        public Health(float maxHealth)
        {
            
            _max.Value = maxHealth;
            _current.Value = maxHealth;
        }

        public void SetMax(float value)
        {
            _max.Value = value;
            _current.Value = Mathf.Min(_current.Value, value);
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
            _current.Value = Mathf.Min(_current.Value + value, _max.Value);
        }
    }
}
