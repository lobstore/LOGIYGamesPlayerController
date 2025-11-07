using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsController : MonoBehaviour
{
    public static PlayerStatsController Instance { get; private set; }

    public HealthModel healthModel { get; private set; }
    public StaminaModel staminaModel{get; private set;}
    public ThirstModel thirstModel{get; private set;}
    public ExperienceModel experienceModel{get; private set;}
    public DefenseModel defenseModel{get; private set;}
    public AttackModel attackModel{get; private set;}
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

        healthModel = GetComponent<HealthModel>();
        staminaModel = GetComponent<StaminaModel>();
        thirstModel = GetComponent<ThirstModel>();
        experienceModel = GetComponent<ExperienceModel>();
        defenseModel = GetComponent<DefenseModel>();
        attackModel = GetComponent<AttackModel>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
