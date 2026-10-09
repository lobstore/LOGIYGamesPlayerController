using System;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames
{
    [Serializable]
    public class ItemSlot
    {
        public readonly UnityEvent OnItemChanged = new();
        [SerializeField] private Item item;
        public Item Item => item;
        public void ClearSlot()
        {
            item = null;
            OnItemChanged.Invoke();
        }

    }
}
