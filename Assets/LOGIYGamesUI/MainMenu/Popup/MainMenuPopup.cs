using UnityEngine;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class MainMenuPopup : PopupBaseMono
    {
        [SerializeField] Button newGameButton;
        [SerializeField] Button exitGameButton;
        [SerializeField] Button loadGameButton;
        [SerializeField] Button continueGameButton;
        protected override void Start()
        {
            exitGameButton.onClick.AddListener(OnExitButtonClick);
            continueGameButton.onClick.AddListener(OnContinueGameClicked);
            DisableButtonsDependingOnData();
            base.Start();
        }
        private void OnContinueGameClicked()
        {
            // save the game anytime before loading a new scene
            if (!DataPersistenceManager.Instance.IsPersistenceDisabled)
            {
                DataPersistenceManager.Instance.SaveGame();

            }
            // load the next scene - which will in turn load the game because of 
            // OnSceneLoaded() in the DataPersistenceManager
            //NetworkManager.Singleton?.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", 15000);
            LevelLoader.Instance.SwitchToScene(2);
        }
        private void OnExitButtonClick()
        {
            Application.Quit();
        }
        private void LateUpdate()
        {
            DisableButtonsDependingOnData();
        }
        private void DisableButtonsDependingOnData()
        {
            if (!DataPersistenceManager.Instance.HasGameData())
            {
                continueGameButton.interactable = false;
                loadGameButton.interactable = false;
            }
        }
        public override void Show()
        {
            DisableButtonsDependingOnData();
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}