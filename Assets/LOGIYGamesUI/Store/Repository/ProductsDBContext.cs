using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    [CreateAssetMenu(menuName = "Databases/ProductsDB", fileName = "ProductsDB")]
    public class ProductsDBContext : ScriptableObject, IDBContext<ProductModel>
    {

        public List<ProductModel> Products = new List<ProductModel>();

        public IEnumerable<ProductModel> GetEntities()
        {
            if (Products == null) { return null; }
            return Products;
        }

    }
}