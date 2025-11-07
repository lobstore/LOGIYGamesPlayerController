using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
[Serializable]
public class Item : ScriptableObject
{
    public bool IsConsumable;
    public string ItemName;           // Название предмета
    public int maxStackSize;          // Максимальное количество предметов в одной ячейке
    public Sprite icon;
    public virtual void Use()
    {
        
    }
}

[Serializable]
public class ResourceItem : Item
{
}
