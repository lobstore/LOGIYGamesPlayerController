using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace LOGIYGames
{
    public class PlayerProfileView : MonoBehaviour
    {
        [SerializeField]
        private UIDocument document;

        private ProgressBar healthFill;
        private ProgressBar staminaFill;
        private Label characterName;
        DisposableBag subscriprions;

        private void Start()
        {
            var root = document.rootVisualElement;

            healthFill = root.Q<ProgressBar>("Healthbar");
            staminaFill = root.Q<ProgressBar>("Staminabar");
            characterName = root.Q<Label>("CharacterName");
            healthFill.lowValue = 0;
            staminaFill.lowValue = 0;
        }

        public void Bind(PlayerProfilePresenter presenter)
        {
            subscriprions.Add(presenter.Health.Subscribe(_val =>
            {
                UpdateHealthBar(_val, presenter.MaxHealth.CurrentValue);
            }));
            subscriprions.Add(presenter.MaxHealth.Subscribe(_val =>
            {
                UpdateHealthBar(presenter.MaxHealth.CurrentValue, _val);
            }));



            subscriprions.Add(presenter.Stamina.Subscribe(_val =>
            {
                UpdateStaminaBar(_val, presenter.MaxStamina.CurrentValue);
            }));

            subscriprions.Add(presenter.MaxStamina.Subscribe(_val =>
            {
                UpdateStaminaBar(presenter.MaxStamina.CurrentValue, _val);
            })); 
            
            subscriprions.Add(presenter.MaxStamina.Subscribe(_val =>
            {
                UpdateCharacterName(presenter.Name.CurrentValue);
            }));
            subscriprions.AddTo(this);
        }
        public void Unbind()
        {
            subscriprions.Dispose();
        }
        private void UpdateHealthBar(float value, float maxValue)
        {
            healthFill.highValue = maxValue;
            healthFill.value = value;
        }
        private void UpdateStaminaBar(float value, float maxValue)
        {
            staminaFill.highValue = maxValue;
            staminaFill.value = value;
        }
        private void UpdateCharacterName(string newName)
        {
             characterName.text = newName;
        }
    }
}
