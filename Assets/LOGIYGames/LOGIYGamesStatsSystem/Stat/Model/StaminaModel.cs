using UnityEngine;
using UnityEngine.Events;

public class StaminaModel : MonoBehaviour
{
    public UnityEvent<float> CurrentValueChanged = new();
    public UnityEvent<float> MaxValueChanged = new();
    [SerializeField] float stamina;
    [SerializeField] float maxStamina;
    public float CurrentValue
    {
        get { return stamina; }
        set { CurrentValueChanged.Invoke(value); if (value < 0) { stamina = 0; } else if (value > maxStamina) { stamina = maxStamina; } else { stamina = value; } }

    }

    public float MaxValue { get => maxStamina; set { maxStamina = value; MaxValueChanged.Invoke(value); } }
}
