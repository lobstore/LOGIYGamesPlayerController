using System;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
namespace LOGIYGames
{
    public class SaveSlotPresenter : IDisposable
    {
        private SaveSlotModel saveSlotModel;
        private SaveSlotView saveSlotView;
        private bool isForLoadingData;

        public delegate void SlotClickedEvent(SaveSlotModel lobbyModel);
        public event SlotClickedEvent OnSlotClickedEvent;
        public delegate void ClearSlotClickedEvent(SaveSlotModel lobbyModel);
        public event ClearSlotClickedEvent OnClearSlotClickedEvent;
        public SaveSlotPresenter(SaveSlotView saveSlotView, SaveSlotModel saveSlotModel, bool isForLoadingData)
        {
            this.saveSlotModel = saveSlotModel;
            this.saveSlotView = saveSlotView;
            this.isForLoadingData = isForLoadingData;
            UpdateView();

            saveSlotView.SlotButton.onClick.AddListener(OnSlotClicked);
            saveSlotView.ClearSlotButton.onClick.AddListener(OnClearSlotClicked);
            saveSlotModel.OnDataChanged.AddListener(UpdateView);
        }

        private void UpdateView()
        {
            if (saveSlotModel.IsEmpty)
            {
                saveSlotView.TextMeshProUGUI.text = "EMPTY";
                saveSlotView.ClearSlotButton.interactable = false;
                if (isForLoadingData)
                {
                    saveSlotView.SlotButton.interactable = false;
                }
                else
                {
                    saveSlotView.SlotButton.interactable = true;
                }
            }
            else
            {
                saveSlotView.TextMeshProUGUI.text = saveSlotModel.Progress.ToString();
                saveSlotView.ClearSlotButton.interactable = true;
                saveSlotView.SlotButton.interactable = true;
            }


        }

        private void OnSlotClicked()
        {
            if (isForLoadingData)
            {
                DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlotModel.ProfileId.ToString());
                SaveGameAndLoadScene();
            }        // case - new game, but the save slot has data
            else if (!saveSlotModel.IsEmpty)
            {
                DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlotModel.ProfileId.ToString());
                DataPersistenceManager.Instance.NewGame();
                SaveGameAndLoadScene();
            }
            // case - new game, and the save slot has no data
            else
            {
                DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlotModel.ProfileId.ToString());
                DataPersistenceManager.Instance.NewGame();
                SaveGameAndLoadScene();
            }
            OnSlotClickedEvent?.Invoke(saveSlotModel);
        }
        private void OnClearSlotClicked()
        {
            if (saveSlotModel.IsEmpty) { return; }

            saveSlotModel.Data = null;
            DataPersistenceManager.Instance.DeleteProfileData(saveSlotModel.ProfileId);

            OnClearSlotClickedEvent?.Invoke(saveSlotModel);
        }
        private void SaveGameAndLoadScene()
        {
            // save the game anytime before loading a new scene
            DataPersistenceManager.Instance.SaveGame();
            // load the next scene - which will in turn load the game because of 
            // OnSceneLoaded() in the DataPersistenceManager
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", 15000);
            LevelLoader.Instance.SwitchToScene(2);
        }
        public void Dispose()
        {
            saveSlotView.SlotButton.onClick.RemoveListener(OnSlotClicked);
            saveSlotView.ClearSlotButton.onClick.RemoveListener(OnClearSlotClicked);
            saveSlotModel.OnDataChanged.RemoveListener(UpdateView);
        }
    }
}