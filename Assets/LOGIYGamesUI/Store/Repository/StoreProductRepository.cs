using System;
using System.Collections.Generic;
using System.Linq;
namespace LOGIYGames
{
    public class StoreProductRepository : IRepository<ProductModel>
    {
        List<ProductModel> productsDb;
        public StoreProductRepository(IDBContext<ProductModel> productsDBContext)
        {
            productsDb = productsDBContext.GetEntities().ToList();
        }

        public ProductModel GetById(Guid id)
        {
            return productsDb.Where(products => { return products.Id == id; }).First();
        }
        public IEnumerable<ProductModel> GetAll()
        {
            return productsDb;
        }

        public void Add(ProductModel item)
        {
            productsDb.Add(item);
        }

        public void Delete(Guid id)
        {
            productsDb.Remove(productsDb.Where(products => { return products.Id == id; }).First());
        }


        public void Update(ProductModel item)
        {
            var existingProduct = GetById(item.Id);
            if (existingProduct != null)
            {
                existingProduct.Sprite = item.Sprite;
                existingProduct.Cost = item.Cost;
                existingProduct.Description = item.Description;
                existingProduct.RequiredLvl = item.RequiredLvl;
            }
        }
    }
}