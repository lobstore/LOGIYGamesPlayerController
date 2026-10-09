using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] List<ItemSlot> slots;
        public IReadOnlyList<ItemSlot> Slots => slots;
    }
}
