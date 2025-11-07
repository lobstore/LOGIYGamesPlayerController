using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    public Image itemIcon;
    public InventorySlot inventorySlot;
    public TextMeshProUGUI quantityText;
    private Transform originalParent;
    private RectTransform dragIcon;
    private Image dragIconImage;
    private Canvas canvas;

    private void Start()
    {
        originalParent = transform;
    }
    /// <summary>
    /// Initialize and bind to <seealso cref="InventorySlot"/>
    /// </summary>
    /// <param name="inventorySlot"></param>
    public void Initialize(InventorySlot inventorySlot, Canvas canvas)
    {
        this.canvas = canvas;
        this.inventorySlot = inventorySlot;

        inventorySlot.OnSlotChanged.AddListener(UpdateView);

        if (inventorySlot.IsEmpty)
        {
            ClearClot();
        }
    }
    public void UpdateView()
    {
        if (inventorySlot.Item == null)
        {
        
            quantityText.text = "0";
            quantityText.enabled = false;
            itemIcon.sprite = null;
            return;
        }

        itemIcon.sprite = inventorySlot.Item.icon;
        quantityText.text = inventorySlot.ItemQuantity.ToString();

        if (inventorySlot.ItemQuantity >= 1)
        {
            quantityText.enabled = true;
        }
        else
        {
            quantityText.enabled = false;
        }
    }
    public void AddItemToInventorySlot(InventorySlot newSlot)
    {
        if (newSlot.Item != null)
        {
            inventorySlot.AddItem(newSlot.Item, newSlot.ItemQuantity);
            UpdateView();
        }
        else
        {
            ClearClot();
        }
    }

    public void ClearClot()
    {
        inventorySlot.ClearSlot();
        itemIcon.sprite = null;
        quantityText.enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventorySlot.IsEmpty)
        {
            return;
        }
        // Создаем временную иконку для перетаскивания
        dragIcon = new GameObject("DragIcon").AddComponent<RectTransform>();
        dragIcon.SetParent(canvas.transform, false);
        dragIcon.sizeDelta = new Vector2(100, 100);  // Размер иконки, можно настроить

        dragIconImage = dragIcon.gameObject.AddComponent<Image>();
        dragIconImage.sprite = itemIcon.sprite;  // Используем ту же иконку
        dragIconImage.raycastTarget = false;  // Отключаем взаимодействие с мышью для временной иконки

        itemIcon.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventorySlot.IsEmpty) { return; }
        if (dragIcon != null)
        {
            dragIcon.position = Input.mousePosition;  // Перемещаем временную иконку за курсором
        }
    }

    public void OnDrop(PointerEventData eventData)
    {

        InventorySlotUI droppedOnSlotUI = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        UpdateActiveSlotInInventory();
        if (droppedOnSlotUI == null)
        {
            return;
        }
        if (droppedOnSlotUI != this && !droppedOnSlotUI.inventorySlot.IsEmpty)
        {
            if (droppedOnSlotUI.inventorySlot.Item != inventorySlot.Item)
            {
                SwapItems(droppedOnSlotUI);
            }
            else
            {
                InventoryManager.Instance.playerInventory.MoveItemToSlot(droppedOnSlotUI.inventorySlot, inventorySlot);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            Destroy(dragIcon.gameObject); 
        }

        itemIcon.enabled = true;  
    }

    private void SwapItems(InventorySlotUI otherSlotUI)
    {

        InventorySlot tempSlot = new InventorySlot(otherSlotUI.inventorySlot.Item, otherSlotUI.inventorySlot.inventory, otherSlotUI.inventorySlot.ItemQuantity);  // Временно сохраняем слот другого предмета

        otherSlotUI.inventorySlot.ClearSlot();
        otherSlotUI.AddItemToInventorySlot(inventorySlot);  // Меняем предметы местами
        inventorySlot.ClearSlot();
        AddItemToInventorySlot(tempSlot);
        otherSlotUI.UpdateView();
        UpdateView();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateActiveSlotInInventory();
    }
    private void UpdateActiveSlotInInventory()
    {
        InventoryManager.Instance.SelectedInventory = inventorySlot.inventory;
        InventoryManager.Instance.SelectedInventory.SelectedSlot = inventorySlot;
    }
}