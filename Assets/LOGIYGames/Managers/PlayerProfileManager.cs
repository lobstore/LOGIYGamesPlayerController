using LOGIYGames.CharacterCore;
using R3;
using System.Xml.Linq;
using UnityEngine;

namespace LOGIYGames
{
    public class PlayerProfileManager : PersistentSingleton<PlayerProfileManager>
    {
        [SerializeField] private PlayerProfileView profileView;
        private PlayerProfilePresenter profilePresenter;
        private ReactiveProperty<string> Name = new();
        private void Start()
        {
            PlayerManager.Instance.OnCharacterChanged.AddListener(UpdateProfileView);
        }
        private void UpdateProfileView(Actor newChar)
        {
            profilePresenter?.Dispose();
            Name.Value = newChar.name;
            profilePresenter = new PlayerProfilePresenter(newChar.HealthController.Health, newChar.StaminaController.Stamina, Name, profileView);
        }
    }
}
