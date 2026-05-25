using LOGIYGames.CharacterCore;
using R3;
using System;
namespace LOGIYGames
{
    public class PlayerHealthPresenter : IDisposable
    {
        public readonly ReactiveProperty<float> Health = new();
        public readonly ReactiveProperty<float> MaxHealth = new();

        PlayerHealthView HealthView;
        DisposableBag DisposableBag;
        public PlayerHealthPresenter(HealthModel health, PlayerHealthView healthView)
        {
            HealthView = healthView;
            DisposableBag.Add(health.CurrentHealth.Subscribe(value =>
            {
                Health.Value = value;
            }));
            DisposableBag.Add(health.MaxHealth.Subscribe(value =>
            {
                MaxHealth.Value = value;
            }));
            HealthView.Bind(this);
        }
        public void Dispose()
        {
            HealthView.Unbind();
            DisposableBag.Dispose();
        }
    }
}
