using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace LOGIYGames
{
    public class StorePopup : PopupBaseMono
    {
        [Tooltip("The Transform where the views will be created")]
        [SerializeField] private Transform viewTransform;
        [SerializeField] private GameObject viewPrefab;

        List<ProductModel> productModels;
        protected override void Start()
        {
            productModels = GameManager.Instance.StoreProductRepository.GetAll().ToList();
            CreateProducts();
            base.Start();
        }

        void CreateProducts()
        {
            foreach (var model in productModels)
            {
                var go = Instantiate(viewPrefab, viewTransform);
                var presenter = new ProductPresenter(go.GetComponent<ProductView>(), model);
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
}