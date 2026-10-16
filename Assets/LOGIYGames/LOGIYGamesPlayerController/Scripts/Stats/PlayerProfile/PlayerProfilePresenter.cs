using LOGIYGames.CharacterCore;
using R3;
using System;
using UnityEngine;
namespace LOGIYGames
{
    public class PlayerProfilePresenter : IDisposable
    {
        public string Name;
        public Health HealthModel { get; private set; }
        public Stamina StaminaModel { get; private set; }
        PlayerProfileView ProfileView;
        public PlayerProfilePresenter(Actor actor, PlayerProfileView profileView)
        {
            HealthModel = actor.HealthController.Health;
            StaminaModel = actor.StaminaController.Stamina;
            ProfileView = profileView;
            ProfileView.Bind(this);
        }
        public void Dispose()
        {
            ProfileView.Unbind();
        }
    }
}
