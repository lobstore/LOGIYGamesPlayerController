using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class RewardedVideoPopup : PopupBaseMono
    {
        [SerializeField] private Button watchWideoButton;
        private List<IResourceModel> rewardResourcesModels;
        [SerializeField] private Transform viewTransform;
        [SerializeField] private GameObject viewPrefab;
        [SerializeField] private Sprite goldSprite;

        protected override void Start()
        {
            rewardResourcesModels = new List<IResourceModel>() { new GoldModel() { Name = "Gold", Quantity = 100, Sprite = goldSprite }, new GoldModel() { Name = "Gold", Quantity = 100, Sprite = goldSprite } };
            CreateRewards();
            watchWideoButton.onClick.AddListener(OnWatchWideoClicked);
            base.Start();
        }

        private void OnWatchWideoClicked()
        {
            //rewarded video invoke
        }
        void CreateRewards()
        {
            foreach (var model in rewardResourcesModels)
            {
                var go = Instantiate(viewPrefab, viewTransform);
                new RewardPresenter(model, go.GetComponent<RewardView>());
            }
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }
    }

    public class RewardPresenter
    {
        IResourceModel model;
        RewardView view;
        public RewardPresenter(IResourceModel model, RewardView view)
        {
            this.model = model;
            this.view = view;

            view.Sprite.sprite = model.Sprite;
            view.Value.text = " X " + model.Quantity.ToString();
        }
    }
    public class GoldModel : IResourceModel
    {
        public UnityEvent OnValueChanged { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public Sprite Sprite { get; set; }
    }
    public interface IResourceModel
    {
        UnityEvent OnValueChanged { get; set; }
        string Name { get; }
        string Description { get; }

        int Quantity { get; }

        Sprite Sprite { get; }
    }
}