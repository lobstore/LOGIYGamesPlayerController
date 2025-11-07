using UnityEngine;
using UnityEngine.Events;

public class ExperienceModel : MonoBehaviour
{
    public UnityEvent<float> CurrentValueChanged = new();
    public UnityEvent<float> MaxValueChanged = new();
    [SerializeField] float experience;
    [SerializeField] float maxExperience;
    public float Experience
    {
        get { return experience; }
        set { if (value < 0) { experience = 0; } else if (value > maxExperience) { experience = maxExperience; } else { experience = value; } CurrentValueChanged.Invoke(value); }
    }

    public float MaxExperience { get => maxExperience; set { maxExperience = value; MaxValueChanged.Invoke(value); } }
}