using System;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace LOGIYGames
{
    public class LobbyCreatePopup : PopupBaseMono
    {
        [SerializeField] GameObject lobbyPrivateCodeInputGO;
        LobbyModel CreatedLobbyModel = null;
        LobbyCreatingView creatingLobbyView;
        LobbyCreatingPresenter lobbyCreatingPresenter = null;
        protected override void Start()
        {
            CreateLobbyCreatingPopup();
            base.Start();

        }
        private void CreateLobbyCreatingPopup()
        {
            CreatedLobbyModel = new LobbyModel();
            creatingLobbyView = GetComponent<LobbyCreatingView>();
            lobbyCreatingPresenter = new LobbyCreatingPresenter(CreatedLobbyModel, creatingLobbyView);
            lobbyCreatingPresenter.OnCreateButtonClickedEvent += ApplyCreatedLobby;
            lobbyCreatingPresenter.OnToggleChangedEvent += ShowCodeInput;
        }
        private async void ApplyCreatedLobby()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CreatedLobbyModel.Name))
                {
                    throw new Exception("Lobby cant be empty");
                }

               // await CreateLobby(CreatedLobbyModel);
                SceneManager.LoadScene(2);
            }
            catch (Exception e)
            {

                Debug.Log(e);
            }
        }

        private void ShowCodeInput(bool isPrivate)
        {
            if (isPrivate)
            {
                lobbyPrivateCodeInputGO.SetActive(true);
            }
            else
            {
                lobbyPrivateCodeInputGO.SetActive(false);
            }
        }

        public override void Hide()
        {
            base.Hide();
        }
        public override void Show()
        {
            base.Show();
        }

        private void OnDestroy()
        {
            lobbyCreatingPresenter.OnToggleChangedEvent -= ShowCodeInput;
            lobbyCreatingPresenter.OnCreateButtonClickedEvent -= ApplyCreatedLobby;
        }
    }
}