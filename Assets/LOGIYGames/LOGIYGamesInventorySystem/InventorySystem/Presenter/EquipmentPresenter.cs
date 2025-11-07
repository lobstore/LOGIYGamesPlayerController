using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPresenter : MonoBehaviour
{

    [Header("Views")]

    [SerializeField] private TextMeshProUGUI defenceStatText;
    [SerializeField] private TextMeshProUGUI damageStatText;
    [SerializeField] private InventorySlotUI subSlotUI;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Slider healthStatusSlider;
    [SerializeField] private Slider hungerStatusSlider;
    [SerializeField] private Slider thirstStatusSlider;

    private DefenseModel defenseModel;
    private AttackModel attackModel;
    HealthModel healthModel;
    StaminaModel hungerModel;
    ThirstModel thirstModel;

    [SerializeField] float animationDuration;
    Inventory playerInventory;

    [SerializeField] Canvas baseCanvas;

    public void Initialize()
    {
    
        healthModel = PlayerStatsController.Instance.healthModel;
        hungerModel = PlayerStatsController.Instance.staminaModel;
        thirstModel = PlayerStatsController.Instance.thirstModel;
        defenseModel = PlayerStatsController.Instance.defenseModel;
        attackModel = PlayerStatsController.Instance.attackModel;

        healthModel.CurrentValueChanged.AddListener(OnHealthChanged);
        hungerModel.CurrentValueChanged.AddListener(OnHungerChanged);
        thirstModel.CurrentValueChanged.AddListener(OnThirstChanged);
        healthStatusSlider.maxValue = healthModel.MaxHealth;
        healthStatusSlider.value = healthModel.CurrentHealth;
        hungerStatusSlider.maxValue = hungerModel.MaxValue;
        hungerStatusSlider.value = hungerModel.CurrentValue;
        thirstStatusSlider.maxValue = thirstModel.MaxThirst;
        thirstStatusSlider.value = thirstModel.Thirst;

        playerInventory = InventoryManager.Instance.playerInventory;
        playerInventory.OnInventoryChanged.AddListener(UpdateSlot);
        subSlotUI.Initialize(playerInventory.SubSlot, baseCanvas);
        UpdateSlot();
    }

    private Coroutine healthChangeCoroutine;
    private Coroutine hungerChangeCoroutine;
    private Coroutine thirstChangeCoroutine;

    private void OnDestroy()
    {
        healthModel.CurrentValueChanged.RemoveListener(OnHealthChanged);
        hungerModel.CurrentValueChanged.RemoveListener(OnHungerChanged);
        thirstModel.CurrentValueChanged.RemoveListener(OnThirstChanged);
    }
    private void OnHealthChanged(float value)
    {
        if (!gameObject.activeInHierarchy) return;
        if (healthChangeCoroutine != null)
        {
            StopCoroutine(healthChangeCoroutine);
        }
        healthChangeCoroutine = StartCoroutine(AnimateValueChange(healthModel.CurrentHealth, healthStatusSlider));
    }

    private void OnHungerChanged(float value)
    {
        if (!gameObject.activeInHierarchy) return;
        if (hungerChangeCoroutine != null)
        {
            StopCoroutine(hungerChangeCoroutine);
        }
        hungerChangeCoroutine = StartCoroutine(AnimateValueChange(hungerModel.CurrentValue, hungerStatusSlider));
    }


    private void OnThirstChanged(float value)
    {
        if (!gameObject.activeInHierarchy) return;
        if (thirstChangeCoroutine != null)
        {
            StopCoroutine(thirstChangeCoroutine);
        }
        thirstChangeCoroutine = StartCoroutine(AnimateValueChange(thirstModel.Thirst, thirstStatusSlider));
    }

    private IEnumerator AnimateValueChange(float value, Slider slider)
    {
        float startValue = slider.value;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, value, elapsed / animationDuration);
            yield return null;
        }

        slider.value = value;
    }
    private void ValueChange(float value, Slider slider)
    {
        slider.value = value;
    }
    public void SetActive(bool isActive)
    {
        GetComponent<Canvas>().enabled=isActive;
    }

    private void UpdateStatus()
    {
        ValueChange(healthModel.CurrentHealth, healthStatusSlider);
        ValueChange(hungerModel.CurrentValue, hungerStatusSlider);
        ValueChange(thirstModel.Thirst, thirstStatusSlider);
    }
    private void UpdateSlot()
    {
        subSlotUI.UpdateView();
    }
}
