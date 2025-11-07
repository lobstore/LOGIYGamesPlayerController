using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPresenter : MonoBehaviour
{
    [Header("Views")]
    [SerializeField] private Button actionButton;
    [SerializeField] private Button discardButton;
    [SerializeField] private Button splitButton;
    [SerializeField] private Transform slotsHolder;
    [SerializeField] private InventorySlotUI[] slots;
    [SerializeField] private TextMeshProUGUI titleText;
    [Header("Models")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Inventory playerInventory;

    private Inventory selectedInventory;

    [SerializeField] Canvas baseCanvas;
    private void Use()
    {
        if (selectedInventory!=null && !selectedInventory.SelectedSlot.IsEmpty)
        selectedInventory?.SelectedSlot?.Use();
    }

    private void Split()
    {
        selectedInventory?.SplitItemInHalf(selectedInventory.SelectedSlot);
    }

    private void Discard()
    {
        if (selectedInventory != null && !selectedInventory.SelectedSlot.IsEmpty)
            selectedInventory.SelectedSlot.ClearSlot();
    }

    public void Initialize()
    {
        playerInventory = InventoryManager.Instance.playerInventory;
        InventoryManager.Instance.OnSelectedInventoryChanged.AddListener(() => { selectedInventory = InventoryManager.Instance.SelectedInventory; });
        playerInventory.OnInventoryChanged.AddListener(UpdateView);
        actionButton.onClick.AddListener(Use);
        splitButton.onClick.AddListener(Split);
        discardButton.onClick.AddListener(Discard);
        slots = new InventorySlotUI[playerInventory.Slots.Count];

        for (int i = 0; i < slots.Length; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotsHolder);
            slots[i] = slotGO.GetComponent<InventorySlotUI>();
            slots[i].Initialize(playerInventory.Slots[i], baseCanvas);

        }
        UpdateView();
    }
    private void UpdateView()
    {
        for (int i = 0; i < playerInventory.Slots.Count; i++)
        {
            if (playerInventory.Slots[i].Item != null)
            {
                slots[i].UpdateView();

            }
            else
            {
                slots[i].ClearClot();
            }
        }

    }

    public void SetActive(bool isActive)
    {
        GetComponent<Canvas>().enabled = isActive;
    }
}
