using LOGIYGames.Timers;
using R3;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class Stamina
    {

        public ReadOnlyReactiveProperty<float> Current => _current;
        public ReadOnlyReactiveProperty<float> Max => _max;

        private readonly ReactiveProperty<float> _current = new();
        private readonly ReactiveProperty<float> _max = new();


        public Subject<float> StaminaUsed = new();
        public Subject<float> StaminaRestored = new();
        public Subject<Unit> Exhausted = new();

        public Stamina(float maxValue)
        {
            _max.Value = maxValue;
            _current.Value = maxValue;
        }
        public void SetMax(float value)
        {
            _max.Value = value;
            _current.Value = Mathf.Min(_current.Value, value);
        }
        public bool CanUse(float amount)
        {
            return _current.CurrentValue >= amount;
        }

        public bool TryUse(float amount)
        {
            if (!CanUse(amount))
            {
                Exhausted.OnNext(Unit.Default);
                return false;
            }

            _current.Value -= amount;

            StaminaUsed.OnNext(amount);

            return true;
        }

        public void Restore(float amount)
        {
            float previous = _current.Value;

            _current.Value =
                Mathf.Min(_current.CurrentValue + amount, _max.CurrentValue);

            float restored = _current.CurrentValue - previous;

            if (restored > 0)
            {
                StaminaRestored.OnNext(restored);
            }
        }


    }

}
