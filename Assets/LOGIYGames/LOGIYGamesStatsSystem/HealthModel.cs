using UnityEngine;
using UnityEngine.Events;

public class HealthModel : MonoBehaviour
{
    public UnityEvent<float> CurrentValueChanged = new();
    public UnityEvent<float> MaxValueChanged = new();
    [SerializeField] float health;
    [SerializeField] float maxHealth;
    public float CurrentHealth
    {
        get { return health; }
        set { if (value < 0) { health = 0; } else if (value > maxHealth) { health = maxHealth; } else { health = value; } CurrentValueChanged.Invoke(health); }
    }

    public float MaxHealth { get => maxHealth; set { maxHealth = value; MaxValueChanged.Invoke(value); } }
}
