using System;
namespace LOGIYGames
{
    public class LobbyCreatingPresenter : IDisposable
    {
        public LobbyModel LobbyModel { get; set; }
        public LobbyCreatingView LobbyCreatingView { get; set; }

        public delegate void OnToggleChanged(bool isPrivate);
        public event OnToggleChanged OnToggleChangedEvent;

        public delegate void OnCreateButtonClicked();
        public event OnCreateButtonClicked OnCreateButtonClickedEvent;
        public LobbyCreatingPresenter(LobbyModel lobbyModel, LobbyCreatingView lobbyCreatingView)
        {
            if (lobbyModel == null)
                lobbyModel = new LobbyModel();
            LobbyModel = lobbyModel;
            LobbyCreatingView = lobbyCreatingView;
            lobbyCreatingView.ApplyingButton.interactable = false;
            lobbyCreatingView.LobbyNameInputField.onValueChanged.AddListener((var) =>
            {
                if (string.IsNullOrWhiteSpace(var))
                {
                    lobbyCreatingView.ApplyingButton.interactable = false;
                }
                else
                {
                    lobbyCreatingView.ApplyingButton.interactable = true;
                }
            });
            lobbyCreatingView.ApplyingButton.onClick.AddListener(() =>
            {
                UpdateModel();
                OnCreateButtonClickedEvent?.Invoke();
                lobbyCreatingView.ApplyingButton.interactable = false;
            });
            lobbyCreatingView.PrivacyToggle.onValueChanged.AddListener((value) => { OnToggleChangedEvent.Invoke(value); });
        }
        private void UpdateModel()
        {
            LobbyModel.Name = LobbyCreatingView.LobbyNameInputField.text;
            LobbyModel.MaxPlayers = (int)LobbyCreatingView.LobbyMembersMaxCount.value;
            LobbyModel.IsPrivate = LobbyCreatingView.PrivacyToggle.isOn;
            LobbyModel.LobbyPrivateCode = LobbyCreatingView.LobbyCodeInputField.text;
        }

        public void Dispose()
        {
            LobbyCreatingView.ApplyingButton.onClick.RemoveAllListeners();
            LobbyCreatingView.PrivacyToggle.onValueChanged.RemoveAllListeners();
        }
    }
}