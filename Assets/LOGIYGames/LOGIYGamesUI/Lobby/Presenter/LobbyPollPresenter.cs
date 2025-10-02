using System;
namespace LOGIYGames
{
    public class LobbyPollPresenter : IDisposable
    {
        private LobbyModel lobbyModel;
        private LobbyItemView lobbyItemView;

        public delegate void ButtonClickedEvent(LobbyModel lobbyModel);
        public event ButtonClickedEvent OnButtonClickedEvent;

        public LobbyPollPresenter(LobbyItemView lobbyItemView, LobbyModel lobbyModel)
        {
            this.lobbyModel = lobbyModel;
            this.lobbyItemView = lobbyItemView;

            UpdateView();

            lobbyItemView.Button.onClick.AddListener(OnButtonClicked);
        }

        private void UpdateView()
        {
            lobbyItemView.LobbyName.text = lobbyModel.Name;
            lobbyItemView.LobbyMembersCount.text = $"{lobbyModel.CurruntPlayersCount} / {lobbyModel.MaxPlayers}";
            if (lobbyModel.IsPrivate)
            {
                lobbyItemView.LobbyPrivacyStatus.text = "Private";
            }
            else
            {
                lobbyItemView.LobbyPrivacyStatus.text = "Public";
            }
        }

        private void OnButtonClicked()
        {
            OnButtonClickedEvent?.Invoke(lobbyModel);
        }

        public void Dispose()
        {
            lobbyItemView.Button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}