using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    [Header("Item Information")]
    public Sprite itemIcon;
    public string itemName;
    [TextArea] public string itemDescription;
    public string itemId;
    public string ownerId;
}
