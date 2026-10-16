using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames.CharacterCore
{
    public class HealthController : MonoBehaviour
    {
        [field: SerializeField][field: ReadOnly] public Health Health { get; private set; }
        Stat VITStat;
        Stat HPStat;
        public UnityEvent<float> OnCurrentHealthChanged;
        public UnityEvent<float> OnMaxHealthChanged;

        public UnityEvent Died;
        Actor Actor;
        private void Awake()
        {
            Actor = GetComponent<Actor>();
            Health = new();
        }
        private void Start()
        {
            VITStat = Actor.Stats.GetStat(StatType.Vitality);
            HPStat = Actor.Stats.GetStat(StatType.BaseHealth);
            UpdateMaxValue();
            Health.Current = Health.Max;
            OnCurrentHealthChanged.Invoke(Health.Current);
        }
        private void LateUpdate()
        {
            UpdateMaxValue();
        }
        public void TakeDamage(DamageData damage)
        {

            float resultDamage = 0;
            switch (damage.ModifierType)
            {
                case ModifierType.Add:
                    resultDamage = damage.Amount;
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
            if (Health.Current <= 0)
                return;

            Health.Current = Mathf.Max(0, Health.Current - value);

            if (Health.Current == 0)
                Died.Invoke();
            OnCurrentHealthChanged?.Invoke(Health.Current);
        }
        private void IncreaseHealth(float value)
        {
            Health.Current = Mathf.Min(Health.Current + value, Health.Max);
            OnCurrentHealthChanged?.Invoke(Health.Current);
        }
        private void UpdateMaxValue()
        {
            Health.Max = HPStat.Value + VITStat.Value * HPStat.Value;
            OnMaxHealthChanged.Invoke(Health.Max);
        }
    }
}
