using UnityEngine;
using UnityEngine.Events;
public class InventorySlot
{
    public Inventory inventory;
    public UnityEvent OnSlotChanged = new();
    [field: SerializeField] public Item Item { get; private set; }
    private bool isSubSlot;
    public bool IsSubSlot { get => isSubSlot; private set => isSubSlot = value; }
    [SerializeField] private int itemQuantity;
    public int ItemQuantity
    {
        get { return itemQuantity; }
        private set => itemQuantity = Mathf.Max(0, value);
    }
    public bool IsEmpty => Item == null;


    public InventorySlot(Item item, Inventory inventory, int quantity, bool isSubslot = false)
    {
        this.inventory = inventory;
        AddItem(item, quantity);
        this.IsSubSlot = isSubslot;
    }
    public void SetItem(Item item, int amount)
    {
        Item = item;
        itemQuantity = amount;
        OnSlotChanged?.Invoke();
    }
    public void AddItem(Item item, int amount = 1)
    {
        if (Item == item)
        {
            ItemQuantity += amount;
        }
        else { Item = item; ItemQuantity = amount; }
        OnSlotChanged?.Invoke();
    }
    public void RemoveItem(int amount = 1)
    {
        ItemQuantity -= amount;

        if (ItemQuantity == 0)
        {
            ClearSlot();
        }

        OnSlotChanged?.Invoke();
    }
    public void Use()
    {
        if (IsEmpty) return;

        Item.Use();

        if (Item.IsConsumable)
        {
            RemoveItem();
        }
        else
        {
            OnSlotChanged?.Invoke();

        }
    }
    public int MaxStackOfItemInSlot()
    {
        return Item.maxStackSize;
    }
    public void ClearSlot()
    {
        Item = null;
        ItemQuantity = 0;
        OnSlotChanged?.Invoke();
    }
}
