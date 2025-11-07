using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public UnityEvent OnSelectedInventoryChanged = new();
    public static InventoryManager Instance { get; private set; }

    public Inventory playerInventory;
    private Inventory selectedInventory;
    public Inventory SelectedInventory { get => selectedInventory; set { selectedInventory = value; OnSelectedInventoryChanged.Invoke(); } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Initialize();
    }


    public bool MoveItemToOtherInventory(Inventory sourceInventory, int sourceSlotIndex, Inventory targetInventory, int targetSlotIndex)
    {

        if (sourceSlotIndex < 0 || sourceSlotIndex >= sourceInventory.Slots.Count)
        {
            Debug.LogError("Индекс исходного слота находится вне диапазона инвентаря");
            return false;
        }


        InventorySlot sourceSlot = sourceInventory.Slots[sourceSlotIndex];


        if (sourceSlot.IsEmpty)
        {
            Debug.Log("Исходный слот пуст");
            return false;
        }


        Item itemToMove = sourceSlot.Item;
        int quantityToMove = sourceSlot.ItemQuantity;


        bool addedSuccessfully = targetInventory.AddItem(itemToMove, quantityToMove, targetSlotIndex);

        if (addedSuccessfully)
        {

            sourceInventory.Slots[sourceSlotIndex].ClearSlot();
            sourceInventory.OnInventoryChanged.Invoke();  
            targetInventory.OnInventoryChanged.Invoke(); 
            return true;
        }
        else
        {
            Debug.Log("Не удалось переместить все предметы в целевой слот");
            return false;
        }
    }

    private void Initialize()
    {
        playerInventory = new Inventory(20);
    }
}