using LOGIYGames.Shared.Data;
using R3;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public interface IDamageable
    {
        void ApplyDamage(DamageData damage);
    }
    public class HealthModule : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private int maxHealth = 100;

        private ReactiveProperty<int> _currentHealth;

        public ReadOnlyReactiveProperty<int> CurrentHealth
            => _currentHealth;

        private Subject<DamageData> _damageTaken = new();
        public Observable<DamageData> DamageTaken => _damageTaken;

        private Subject<Unit> _died = new();
        public Observable<Unit> Died => _died;

        private void Awake()
        {
            _currentHealth = new ReactiveProperty<int>(maxHealth);
        }

        public void ApplyDamage(DamageData damage)
        {
            if (_currentHealth.Value <= 0)
                return;

            _currentHealth.Value -= damage.Amount;

            _damageTaken.OnNext(damage);

            if (_currentHealth.Value <= 0)
            {
                _currentHealth.Value = 0;
                _died.OnNext(Unit.Default);
            }
        }
    }
}
