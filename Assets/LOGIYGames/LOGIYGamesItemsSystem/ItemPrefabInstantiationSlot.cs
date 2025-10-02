using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPrefabInstantiationSlot : MonoBehaviour
{
    public ItemPrefabSlot itemPrefabSlot;
    public GameObject currentItemPrefab;

    public void UnloadItem()
    {
        if (currentItemPrefab != null)
        {
            currentItemPrefab.SetActive(false);
            Destroy(currentItemPrefab);
        }
    }

    public void LoadItem(GameObject itemPrefab)
    {
        currentItemPrefab = itemPrefab;
        itemPrefab.transform.SetParent(transform);
        itemPrefab.transform.localPosition = Vector3.zero;
        itemPrefab.transform.localRotation = Quaternion.identity;
        itemPrefab.transform.localScale = Vector3.one;
    }
}
