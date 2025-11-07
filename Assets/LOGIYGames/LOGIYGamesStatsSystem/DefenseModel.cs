using UnityEngine;
using UnityEngine.Events;

public class DefenseModel : MonoBehaviour
{
    public UnityEvent<float> CurrentValueChanged = new();
    [SerializeField] float defence;
    public float Defence
    {
        get { return defence; }
        set { if (value < 0) { defence = 0; } else { defence = value; } CurrentValueChanged.Invoke(value); }
    }
}
