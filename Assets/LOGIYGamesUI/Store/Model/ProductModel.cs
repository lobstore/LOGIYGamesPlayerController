using System;
using UnityEngine;
namespace LOGIYGames
{
    [Serializable]
    public class ProductModel
    {
        public Guid Id;
        public string Description;
        public int Cost;
        public int RequiredLvl;
        public Sprite Sprite;
    }
}