using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class SavesSlotsPopup : PopupBaseMono
    {
        [SerializeField] Transform viewContainer;
        [SerializeField] GameObject viewPrefab;
        [SerializeField] Button backButton;
        List<SaveSlotModel> saveSlotsModels = new();
        List<SaveSlotView> saveSlotsViews = new();
        List<SaveSlotPresenter> saveSlotsPresenters = new();
        public int saveSlotsNumber;
        public bool IsLoading { get; private set; }

        public override void Show()
        {
            UpdateSaveSlotsList(); // Обновляем список при показе
            base.Show();
        }

        private void UpdateSaveSlotsList()
        {
            // Удаляем старые презентеры
            ClearPresenters();

            // load all of the profiles that exist
            Dictionary<string, GameData> profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();


            for (int i = 0; i < saveSlotsNumber; i++)
            {
                GameData profileData = null;
                saveSlotsModels.Add(new SaveSlotModel { ProfileId = i.ToString() });
                profilesGameData.TryGetValue(saveSlotsModels[i].ProfileId.ToString(), out profileData);
                saveSlotsModels[i].Data = profileData;
                var view = Instantiate(viewPrefab, viewContainer).GetComponent<SaveSlotView>();
                saveSlotsViews.Add(view);

                saveSlotsPresenters.Add(new SaveSlotPresenter(saveSlotsViews[i], saveSlotsModels[i], IsLoading));
            }

        }

        public override void Hide()
        {
            ClearPresenters(); // Очищаем презентеры при скрытии попапа
            base.Hide();
        }
        public void SetMode(bool isLoading)
        {
            IsLoading = isLoading;
        }
        private void ClearPresenters()
        {
            foreach (var presenter in saveSlotsPresenters)
            {
                presenter.Dispose();
            }
            saveSlotsModels.Clear();
            saveSlotsViews.Clear();
            saveSlotsPresenters.Clear();
        }
    }
}