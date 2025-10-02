using SaveIsEasy;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool IsForLoading { get; private set; }

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
            List<SceneFile>profilesGameData = SaveIsEasyAPI.ListOfValidSaves().ToList();


            for (int i = 0; i < profilesGameData.Count; i++)
            {
                saveSlotsModels.Add(new SaveSlotModel() { Data = profilesGameData[i] , SlotId = i });
                var view = Instantiate(viewPrefab, viewContainer).GetComponent<SaveSlotView>();
                saveSlotsViews.Add(view);

                saveSlotsPresenters.Add(new SaveSlotPresenter(saveSlotsViews[i], saveSlotsModels[i], IsForLoading));
            }

        }

        public override void Hide()
        {
            ClearPresenters(); // Очищаем презентеры при скрытии попапа
            base.Hide();
        }
        public void SetForLoading(bool isLoading)
        {
            IsForLoading = isLoading;
        }
        private void ClearPresenters()
        {
            foreach (var presenter in saveSlotsPresenters)
            {
                presenter.Dispose();
            }
            foreach (var view in saveSlotsViews)
            {
                Destroy(view.gameObject);
            }
            saveSlotsModels.Clear();
            saveSlotsViews.Clear();
            saveSlotsPresenters.Clear();
        }
    }
}