using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class GameMenuPopup : PopupBaseMono
    {
        [SerializeField] Button ExitLobbyButton;

        private void Awake()
        {
            //GameControlInputManager.Instance.Exited.AddListener(ChangeState);
            ExitLobbyButton.onClick.AddListener(OnExitLobbyClicked);
        }
        protected override void Start()
        {
            base.Start();
        }
        private void OnDestroy()
        {
            //GameControlInputManager.Instance.Exited.RemoveListener(ChangeState);
        }
        public override void Hide()
        {
            ExitLobbyButton.onClick.RemoveListener(OnExitLobbyClicked);
            base.Hide();
        }
        public override void Show()
        {
            ExitLobbyButton.onClick.AddListener(OnExitLobbyClicked);
            base.Show();
        }
        private void ChangeState()
        {
            if (IsShowing)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
        private async void OnExitLobbyClicked()
        {
            //await LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            await SceneManager.LoadSceneAsync(1);
        }
    }
}