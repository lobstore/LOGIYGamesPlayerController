using LOGIYGames.CharacterCore;
using R3;
using System;
using UnityEngine;
namespace LOGIYGames
{
    public class PlayerProfilePresenter : IDisposable
    {
        public readonly ReactiveProperty<float> Health = new();
        public readonly ReactiveProperty<float> Stamina = new();
        public readonly ReactiveProperty<float> MaxHealth = new();
        public readonly ReactiveProperty<float> MaxStamina = new();
        public readonly ReactiveProperty<string> Name = new();

        PlayerProfileView ProfileView;
        DisposableBag DisposableBag;
        public PlayerProfilePresenter(HealthModule health, StaminaModule stamina, ReactiveProperty<string> name, PlayerProfileView profileView)
        {
            ProfileView = profileView;
            DisposableBag.Add(health.CurrentHealth.Subscribe(value =>
            {
                Health.Value = value;
            }));
            DisposableBag.Add(health.MaxHealth.Subscribe(value =>
            {
                MaxHealth.Value = value;
            }));
            DisposableBag.Add(stamina.CurrentStamina.Subscribe(value =>
            {
                Stamina.Value = value;
            }));
            DisposableBag.Add(stamina.MaxStamina.Subscribe(value =>
            {
                MaxStamina.Value = value;
            }));
            DisposableBag.Add(name.Subscribe(value =>
            {
                Name.Value = value;
            }));
            ProfileView.Bind(this);
        }
        public void Dispose()
        {
            ProfileView.Unbind();
            DisposableBag.Dispose();
        }
    }
}
