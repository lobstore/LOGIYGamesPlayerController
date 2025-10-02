using UnityEngine;
namespace LOGIYGames
{
    public class PlayerLvlPopup : PopupBaseMono
    {
        [SerializeField] private Transform viewTransform;
        [SerializeField] private GameObject viewPrefab;

        private PlayerLevelModel playerLevelModel;
        protected override void Start()
        {
            // playerLevelModel = PlayerDataRepository.Instance.levelModel;
            CreateProduct();
            base.Start();
        }

        void CreateProduct()
        {
            var go = Instantiate(viewPrefab, viewTransform);
            var presenter = new PlayerLevelPresenter(playerLevelModel, go.GetComponent<PlayerLevelView>());
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
}