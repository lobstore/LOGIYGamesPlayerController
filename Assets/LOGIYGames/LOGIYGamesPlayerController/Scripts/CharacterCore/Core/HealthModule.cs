using LOGIYGames.Shared.Data;
using R3;
using System;
using Unity.Properties;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class HealthModule : MonoBehaviour
    {
        [SerializeField] private float maxHealth;
        public SerializableReactiveProperty<float> MaxHealth { get; private set; } = new();

        public SerializableReactiveProperty<float> CurrentHealth {  get; private set; } = new();

        private void Awake()
        {
            MaxHealth.Value = maxHealth;
            CurrentHealth.Value = MaxHealth.CurrentValue;
        }

        public void ApplyDamage(float amount)
        {
            if (CurrentHealth.CurrentValue <= 0)
                return;

            CurrentHealth.Value -= amount;
            Debug.Log($"Took {amount} damage");
            if (CurrentHealth.CurrentValue <= 0)
            {
                CurrentHealth.Value = 0;
            }
        }
        public void ApplyHeal(float amount)
        {
            if (CurrentHealth.CurrentValue > MaxHealth.CurrentValue)
                return;

            CurrentHealth.Value += amount;

        }

    }
}
