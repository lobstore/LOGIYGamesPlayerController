using UnityEngine;
using UnityEngine.Events;

public class AttackModel:MonoBehaviour
{
    public UnityEvent<float> CurrentValueChanged = new();
    [SerializeField] float attack;
    public float Attack
    {
        get { return attack; }
        set { if (value < 0) { attack = 0; } else { attack = value; } CurrentValueChanged.Invoke(value); }
    }
}