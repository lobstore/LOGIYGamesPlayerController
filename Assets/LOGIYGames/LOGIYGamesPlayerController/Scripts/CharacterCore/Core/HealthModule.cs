using LOGIYGames.Shared.Data;
using R3;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class HealthModule : MonoBehaviour
    {
        [SerializeField] private float maxHealth;
        public ReactiveProperty<float> MaxHealth { get; private set; }

        public ReactiveProperty<float> CurrentHealth {  get; private set; }


        private Subject<DamageData> _damageTaken = new();
        private Subject<float> _healTaken = new();
        public Observable<DamageData> OnDamageTaken => _damageTaken.AsObservable();
        public Observable<float> HealTaken => _healTaken.AsObservable();

        private Subject<Unit> _died = new();
        public Observable<Unit> Died => _died;

        private void Awake()
        {
            MaxHealth = new ReactiveProperty<float>(maxHealth);
            CurrentHealth = new ReactiveProperty<float>(MaxHealth.CurrentValue);
        }

        public void ApplyDamage(DamageData damage)
        {
            if (CurrentHealth.Value <= 0)
                return;

            CurrentHealth.Value -= damage.Amount;

            _damageTaken.OnNext(damage);

            if (CurrentHealth.Value <= 0)
            {
                CurrentHealth.Value = 0;
                _died.OnNext(Unit.Default);
            }
        }
        public void ApplyHeal(int amount)
        {
            if (CurrentHealth.Value > maxHealth)
                return;

            _healTaken.OnNext(amount);
            CurrentHealth.Value += amount;

        }
    }
}
