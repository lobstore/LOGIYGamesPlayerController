using UnityEngine;
using UnityEngine.Events;

public class ThirstModel : MonoBehaviour
{
    public UnityEvent<float> CurrentValueChanged = new();
    public UnityEvent<float> MaxValueChanged = new();
    [SerializeField] float thirst;
    [SerializeField] float maxThirst;
    public float Thirst
    {
        get { return thirst; }
        set { if (value < 0) { thirst = 0; } else if (value > maxThirst) { thirst = maxThirst; } else { thirst = value; } CurrentValueChanged.Invoke(value); }

    }

    public float MaxThirst { get => maxThirst; set { maxThirst = value; MaxValueChanged.Invoke(value); } }
}
