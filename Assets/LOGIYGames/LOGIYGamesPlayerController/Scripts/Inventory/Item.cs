using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public class Item
    {
        public ItemData Data;
        public string Name => Data.Name;
        public int StackSize;
        public Sprite Icon => Data.Icon;
        public bool Stackable => Data.IsStackable;
        public int MaxStackSize => Data.MaxStackSize;

    }
}
