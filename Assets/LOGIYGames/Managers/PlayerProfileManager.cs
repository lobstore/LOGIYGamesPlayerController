using LOGIYGames.CharacterCore;
using R3;
using UnityEngine;

namespace LOGIYGames
{
    public class PlayerProfileManager : PersistentSingleton<PlayerProfileManager>
    {
        [SerializeField] private PlayerProfileView profileView;
        private PlayerProfilePresenter profilePresenter;
        override protected void Awake()
        {
            base.Awake();
            PlayerManager.Instance.OnCharacterChanged.AddListener(UpdateProfilePresenter);
        }
        private void UpdateProfilePresenter(Actor newChar)
        {
            profilePresenter?.Dispose();
            profilePresenter = new PlayerProfilePresenter(newChar, profileView);
        }
    }
}
