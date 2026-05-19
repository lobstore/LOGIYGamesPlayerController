using LOGIYGames.Timers;
using R3;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class StaminaModule : MonoModuleBase
    {
        [Header("Settings")]
        [SerializeField]
        private float maxStamina = 100f;

        [SerializeField]
        private float regenPerSecond = 20f;

        [SerializeField]
        private float regenDelay = 1.5f;

        private ReactiveProperty<float> _currentStamina;

        public ReadOnlyReactiveProperty<float> CurrentStamina
            => _currentStamina;

        public float Normalized
            => _currentStamina.Value / maxStamina;

        public Subject<float> StaminaUsed = new();
        public Subject<float> StaminaRestored = new();
        public Subject<Unit> Exhausted = new();

        private CountdownTimer _regenDelayTimer;

        private void Awake()
        {
            _currentStamina = new ReactiveProperty<float>(maxStamina);

            _regenDelayTimer = new CountdownTimer(regenDelay);

            _regenDelayTimer.OnTimerStop += BeginRegen;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (CanRegenerate())
            {
                Regenerate(deltaTime);
            }
        }

        public bool CanUse(float amount)
        {
            return _currentStamina.Value >= amount;
        }

        public bool TryUse(float amount)
        {
            if (!CanUse(amount))
            {
                Exhausted.OnNext(Unit.Default);
                return false;
            }

            _currentStamina.Value -= amount;

            StaminaUsed.OnNext(amount);

            RestartRegenDelay();

            return true;
        }

        public void Restore(float amount)
        {
            float previous = _currentStamina.Value;

            _currentStamina.Value =
                Mathf.Min(_currentStamina.Value + amount, maxStamina);

            float restored = _currentStamina.Value - previous;

            if (restored > 0)
            {
                StaminaRestored.OnNext(restored);
            }
        }

        private void RestartRegenDelay()
        {
            _regenDelayTimer.Stop();
            _regenDelayTimer.Start();
        }

        private bool CanRegenerate()
        {
            return !_regenDelayTimer.IsRunning
                   && _currentStamina.Value < maxStamina;
        }

        private void Regenerate(float time)
        {
            Restore(regenPerSecond * time);
        }

        private void BeginRegen()
        {
            Debug.Log("Stamina regen started");
        }

        private void OnDestroy()
        {
            _regenDelayTimer.Dispose();
        }
    }
}
