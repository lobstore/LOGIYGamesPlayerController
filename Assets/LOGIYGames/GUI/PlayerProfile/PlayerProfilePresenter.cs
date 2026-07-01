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
        public PlayerProfilePresenter(Health health, Stamina stamina, ReactiveProperty<string> name, PlayerProfileView profileView)
        {
            ProfileView = profileView;
            DisposableBag.Add(health.Current.Subscribe(value =>
            {
                Health.Value = value;
            }));
            DisposableBag.Add(health.Max.Subscribe(value =>
            {
                MaxHealth.Value = value;
            }));
            DisposableBag.Add(stamina.Current.Subscribe(value =>
            {
                Stamina.Value = value;
            }));
            DisposableBag.Add(stamina.Max.Subscribe(value =>
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
