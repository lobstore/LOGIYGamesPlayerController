using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class LobbyPollPopup : PopupBaseMono
    {
        [SerializeField] private Transform viewContainer;
        [SerializeField] private GameObject lobbyPreviewPrefab;
        [SerializeField] private Button joinLobbyButton; // Добавим кнопку Join Lobby

        private List<LobbyModel> lobbiesModels = new();
        private List<LobbyPollPresenter> lobbiesPresenters = new();
        private float updateTimer;
        [SerializeField] private float updateTime;
        private LobbyModel selectedLobbyModel = null;

        protected override void Start()
        {
            base.Start();
            joinLobbyButton.interactable = false; // Кнопка изначально неактивна
            joinLobbyButton.onClick.AddListener(JoinLobby);
        }

        public override void Hide()
        {
            base.Hide();
            ClearPresenters(); // Очищаем презентеры при скрытии попапа
        }

        public override void Show()
        {
            base.Show();
            UpdateLobbiesList(); // Обновляем список при показе
        }

        private void Update()
        {
            if (!IsShowing) return;
            updateTimer += Time.deltaTime;
            if (updateTimer >= updateTime)
            {
                updateTimer = 0;
                UpdateLobbiesList();
            }
        }

        private async void UpdateLobbiesList()
        {
            var newLobbies = await GetUpdatedLobbies();

            // Удаляем старые презентеры
            ClearPresenters();

            // Создаем новые презентеры для актуальных лобби
            foreach (var lobby in newLobbies)
            {
                var lobbyViewObj = Instantiate(lobbyPreviewPrefab, viewContainer);
                var lobbyView = lobbyViewObj.GetComponent<LobbyItemView>();
                var presenter = new LobbyPollPresenter(lobbyView, lobby);

                presenter.OnButtonClickedEvent += OnLobbySelected;
                lobbiesPresenters.Add(presenter);
            }

            lobbiesModels = newLobbies;

            // Проверяем, если выбранное лобби больше не в списке - сбрасываем выбор
            if (selectedLobbyModel != null && !lobbiesModels.Any(x => selectedLobbyModel.LobbyId == x.LobbyId))
            {
                selectedLobbyModel = null;
                joinLobbyButton.interactable = false;
            }
        }

        private void OnLobbySelected(LobbyModel lobbyModel)
        {
            selectedLobbyModel = lobbyModel;
            joinLobbyButton.interactable = true;
        }

        private async void JoinLobby()
        {
            try
            {
                if (selectedLobbyModel != null)
                {
                    joinLobbyButton.interactable = false;
                    //await JoinLobby(selectedLobbyModel);
                    await SceneManager.LoadSceneAsync(2);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }


        }

        private void ClearPresenters()
        {
            if (lobbiesPresenters.Count > 0)
            {
                foreach (var presenter in lobbiesPresenters)
                {
                    presenter.OnButtonClickedEvent -= OnLobbySelected;
                    presenter.Dispose(); // Добавим метод Dispose в презентер для очистки
                }

                lobbiesPresenters.Clear();
            }
            if (viewContainer != null)
            {

                if (viewContainer.childCount > 0)
                {
                    foreach (Transform child in viewContainer)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }

        private async Task<List<LobbyModel>> GetUpdatedLobbies()
        {
            return null;
            //return await GetLobbiesListAsync();
        }
        private void OnDestroy()
        {
            joinLobbyButton.onClick.RemoveAllListeners();
        }
    }
}