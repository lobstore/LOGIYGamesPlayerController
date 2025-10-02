namespace LOGIYGames
{
    public class ProductPresenter
    {
        private ProductView productView;
        private ProductModel model;

        public ProductPresenter(ProductView productView, ProductModel model)
        {
            this.productView = productView;
            this.model = model;

            this.productView.buyButton.onClick.AddListener(() => { });
            this.productView.Descrtiption.text = model.Description;
            this.productView.Cost.text = model.Cost.ToString();
            this.productView.Image.sprite = model.Sprite;
        }
    }
}