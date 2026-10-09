using LOGIYGames.CharacterCore;
using R3;
using System;
using UnityEngine;
namespace LOGIYGames
{
    public class PlayerProfilePresenter : IDisposable
    {
        public readonly ReactiveProperty<string> Name = new();
        public Health HealthModel { get; private set; }
        public Stamina StaminaModel { get; private set; }
        PlayerProfileView ProfileView;
        DisposableBag DisposableBag;
        public PlayerProfilePresenter(Health health, Stamina stamina, ReactiveProperty<string> name, PlayerProfileView profileView)
        {
            HealthModel = health;
            StaminaModel = stamina;
            ProfileView = profileView;
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
