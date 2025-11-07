using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public class Inventory
{
    public UnityEvent OnInventoryChanged = new();
    [field:SerializeField] public List<InventorySlot> Slots { get; private set; }
    public int Capacity { get; private set; }

    public InventorySlot SelectedSlot;

    public InventorySlot SubSlot ;

    public Inventory(int capacity)
    {
        Capacity = capacity;
        Slots = new List<InventorySlot>(capacity);
        for (int i = 0; i < Capacity; i++)
        {
            Slots.Add(new InventorySlot(null, this, 0));
        }
        SubSlot = new InventorySlot(null,this, 0, true);
        OnInventoryChanged.Invoke();
    }
    public bool AddItem(Item item, int quantity)
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            if (!Slots[i].IsEmpty && Slots[i].Item == item && Slots[i].ItemQuantity < item.maxStackSize)
            {
                int spaceLeft = item.maxStackSize - Slots[i].ItemQuantity;
                if (quantity <= spaceLeft)
                {
                    Slots[i].AddItem(item ,quantity);
                    OnInventoryChanged.Invoke();
                    return true;
                }
                else
                {
                    Slots[i].AddItem(item, item.maxStackSize);
                    quantity -= spaceLeft;
                }
            }
        }

        while (quantity > 0)
        {
            InventorySlot freeSlot = GetFreeSlot();
            if (freeSlot != null)
            {
                int amountToAdd = Mathf.Min(item.maxStackSize, quantity);
                freeSlot.AddItem(item, amountToAdd);
                quantity -= amountToAdd;
            }
            else
            {
                OnInventoryChanged.Invoke();
                Debug.Log("Инвентарь полон");
                return false;
            }
        }
        OnInventoryChanged.Invoke();
        return true;
    }
    public bool SplitItemInHalf(InventorySlot sourceSlot)
    {

        if (sourceSlot == null || sourceSlot.IsEmpty || sourceSlot.ItemQuantity <= 1)
        {
            Debug.Log("Невозможно разделить предмет: слот пуст или недостаточно предметов для разделения.");
            return false;
        }
        if (sourceSlot.IsSubSlot) return false;

        int amountToSplit = sourceSlot.ItemQuantity / 2;

        InventorySlot freeSlot = GetFreeSlot();
        if (freeSlot == null)
        {
            Debug.Log("Недостаточно свободных слотов для разделения предметов.");
            return false;
        }

        sourceSlot.RemoveItem(amountToSplit);
        freeSlot.AddItem( sourceSlot.Item, amountToSplit);

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool AddItem(Item item, int quantity, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= Slots.Count)
        {
            Debug.LogError("Индекс слота находится вне диапазона инвентаря");
            return false;
        }

        InventorySlot targetSlot = Slots[slotIndex];

        if (!targetSlot.IsEmpty && targetSlot.Item != item)
        {
            Debug.Log("В выбранном слоте уже находится другой предмет");
            return false;
        }

        if (targetSlot.Item == item)
        {
            int spaceLeft = item.maxStackSize - targetSlot.ItemQuantity;

            if (quantity <= spaceLeft)
            {
                targetSlot.AddItem(item, quantity);
                OnInventoryChanged?.Invoke();
                return true;
            }
            else
            {
                targetSlot.AddItem(item,item.maxStackSize);
                quantity -= spaceLeft;
            }
        }

        if (targetSlot.IsEmpty)
        {
            int amountToAdd = Mathf.Min(item.maxStackSize, quantity);
            targetSlot.AddItem(item, amountToAdd);
            quantity -= amountToAdd;

            OnInventoryChanged?.Invoke();
            return true;
        }

        OnInventoryChanged?.Invoke();
        return false;
    }
    public void RemoveItem(InventorySlot item) {
        if (item == null) return;
        if (item.IsSubSlot) {
            SubSlot.ClearSlot();
        }
       Slots[ Slots.IndexOf(item)].ClearSlot();
        OnInventoryChanged?.Invoke();
    }
    private InventorySlot GetFreeSlot()
    {
        return Slots.FirstOrDefault(slot => slot.IsEmpty);
    }

    public bool MoveItemToSlot(InventorySlot sourceSlot, InventorySlot targetSlot)
    {
        if (sourceSlot.IsEmpty)
        {
            Debug.Log("Исходный слот пуст.");
            return false;
        }

        if (targetSlot.IsEmpty)
        {
            targetSlot.AddItem(sourceSlot.Item, sourceSlot.ItemQuantity);
            sourceSlot.ClearSlot();
            OnInventoryChanged.Invoke(); 
            return true;
        }

        if (sourceSlot.Item == targetSlot.Item && targetSlot.ItemQuantity < targetSlot.MaxStackOfItemInSlot())
        {
            int totalQuantity = sourceSlot.ItemQuantity + targetSlot.ItemQuantity;

            if (totalQuantity <= targetSlot.MaxStackOfItemInSlot())
            {
                targetSlot.SetItem(sourceSlot.Item, totalQuantity);
                sourceSlot.ClearSlot();
            }
            else
            {
                int itemsToMove = targetSlot.MaxStackOfItemInSlot() - targetSlot.ItemQuantity;
                targetSlot.AddItem(sourceSlot.Item, itemsToMove);
                sourceSlot.RemoveItem( itemsToMove);
            }

            OnInventoryChanged.Invoke();
            return true;
        }

        SwapItems(sourceSlot, targetSlot);
        return true;
    }

    private void SwapItems(InventorySlot slot1, InventorySlot slot2)
    {
        
        Item tempItem = slot1.Item;
        int tempQuantity = slot1.ItemQuantity;

        slot1.SetItem(slot2.Item, slot2.ItemQuantity);

        slot2.SetItem(tempItem, tempQuantity);
        OnInventoryChanged.Invoke(); 
    }

}
