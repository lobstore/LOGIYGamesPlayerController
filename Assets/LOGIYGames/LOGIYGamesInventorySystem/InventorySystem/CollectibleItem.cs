using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public Item item;  // Предмет, который этот объект представляет
    public int quantity = 1;  // Количество предметов, которое будет добавлено в инвентарь

    // Метод, вызываемый при взаимодействии с игроком
    public void Collect()
    {
        bool addedSuccessfully = InventoryManager.Instance.playerInventory.AddItem(item, quantity);

        if (addedSuccessfully)
        {
            Destroy(gameObject);  // Уничтожаем предмет на сцене после добавления в инвентарь
        }
        else
        {
            Debug.Log("Не удалось добавить предмет. Инвентарь полон.");
        }
    }
}