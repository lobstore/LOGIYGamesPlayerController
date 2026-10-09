using LOGIYGames.CharacterCore;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    public class PlayerInventoryManager : PersistentSingleton<PlayerInventoryManager>
    {
        [SerializeField] private GameObject inventorySlotPrefab;
        [SerializeField] private RectTransform inventoryContainer;
        InventoryUIFactory Factory;

        override protected void Awake()
        {
            base.Awake();
            PlayerManager.Instance.OnCharacterChanged.AddListener(UpdateInventoryUI);
        }

        private void UpdateInventoryUI(Actor newChar)
        {
            Factory = new InventoryUIFactory(newChar.GetComponent<Inventory>(), inventoryContainer, inventorySlotPrefab);
            Factory.Create();
        }
    }
    public class InventoryUIFactory
    {
        private List<ItemSlotPresenter> slotsPresenters = new();
        private List<ItemSlotView> slotsViews = new();
        private GameObject inventorySlotPrefab;
        private RectTransform inventoryContainer;
        Inventory Inventory;

        public InventoryUIFactory(Inventory inventory, RectTransform inventoryContainer, GameObject inventorySlotPrefab)
        {
            Inventory = inventory;
            this.inventorySlotPrefab = inventorySlotPrefab;
            this.inventoryContainer = inventoryContainer;
        }
        public void Create()
        {
            for (int i = 0; i < inventoryContainer.childCount; i++)
            {
                GameObject.Destroy(inventoryContainer.GetChild(i).gameObject);
            }
            if (slotsPresenters.Count > 0)
            {
                slotsPresenters.Clear();
            }
            if (slotsViews.Count > 0)
            {
                foreach (var item in slotsViews)
                {
                    GameObject.Destroy(item);
                }
                slotsViews.Clear();
            }

            if (Inventory != null)
            {
                if (Inventory.Slots.Count <= 0) return;
                foreach (var item in Inventory.Slots)
                {
                    var obj = GameObject.Instantiate(inventorySlotPrefab);
                    obj.transform.SetParent(inventoryContainer, true);
                    obj.transform.localScale = Vector3.one;
                    var view = obj.GetComponent<ItemSlotView>();
                    slotsViews.Add(view);
                    slotsPresenters.Add(new ItemSlotPresenter(view, item));

                }
                foreach (var item in slotsPresenters)
                {
                    item.Bind(Inventory);
                }
            }
        }
    }
}
