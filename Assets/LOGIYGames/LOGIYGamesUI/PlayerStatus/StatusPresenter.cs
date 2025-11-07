using LOGIYGames;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatusPresenter
{
    HealthModel healthModel;
    StaminaModel staminaModel;
    ThirstModel thirstModel;
    ExperienceModel experienceModel;


    StatusView statusView;



    public StatusPresenter(StatusView statusView)
    {
        this.statusView = statusView;

        Initialize();
    }

    public void Initialize()
    {
        healthModel = PlayerStatsController.Instance.healthModel;
        staminaModel = PlayerStatsController.Instance.staminaModel;
        thirstModel = PlayerStatsController.Instance.thirstModel;
        experienceModel = PlayerStatsController.Instance.experienceModel;

        healthModel?.CurrentValueChanged.AddListener(statusView.OnHealthChanged);
        staminaModel?.CurrentValueChanged.AddListener(statusView.OnHungerChanged);
        thirstModel?.CurrentValueChanged.AddListener(statusView.OnThirstChanged);
        experienceModel?.CurrentValueChanged.AddListener(statusView.OnExpirienceChanged);
        statusView. healthStatusSlider.maxValue = healthModel.MaxHealth;
        statusView.healthStatusSlider.value = healthModel.CurrentHealth;
        statusView.hungerStatusSlider.maxValue = staminaModel.MaxValue;
        statusView.hungerStatusSlider.value = staminaModel.CurrentValue;
        statusView.thirstStatusSlider.maxValue = thirstModel.MaxThirst;
        statusView.thirstStatusSlider.value = thirstModel.Thirst;
        statusView.experienceStatusSlider.maxValue = experienceModel.MaxExperience;
        statusView.experienceStatusSlider.value = experienceModel.Experience;
    }
    private void OnDestroy()
    {
        healthModel.CurrentValueChanged.RemoveListener(statusView.OnHealthChanged);
        staminaModel.CurrentValueChanged.RemoveListener(statusView.OnHungerChanged);
        thirstModel.CurrentValueChanged.RemoveListener(statusView.OnThirstChanged);
    }

}