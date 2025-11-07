using UnityEngine;
[RequireComponent(typeof(HealthModel))]
[RequireComponent(typeof(StaminaModel))]
[RequireComponent(typeof(ThirstModel))]

public class ExhaustionController : MonoBehaviour
{
    [SerializeField] public float interval;

    [SerializeField] float healthDecreaseAmount;
    [SerializeField] float hungerDecreaseAmount;
    [SerializeField] float thirstDecreaseAmount;
    private HealthModel healthModel;
    private ThirstModel thirstModel;
    private float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        healthModel = GetComponent<HealthModel>();
        thirstModel = GetComponent<ThirstModel>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            DecreaseThirst();
            timer = 0f;
        }
    }

    private void DecreaseThirst()
    {
        if (thirstModel.Thirst > 0)
        {
            thirstModel.Thirst -= thirstDecreaseAmount;
        }
        else
        {
            DecreaseHealth();
        }
    }


    private void DecreaseHealth()
    {
        healthModel.CurrentHealth -= healthDecreaseAmount;
    }

    private void IncreaseHealth()
    {
        healthModel.CurrentHealth -= healthDecreaseAmount;
    }
}
