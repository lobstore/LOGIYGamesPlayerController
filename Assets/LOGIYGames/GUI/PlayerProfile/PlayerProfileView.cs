using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LOGIYGames
{
    public class PlayerProfileView : MonoBehaviour
    {

        [SerializeField] private Slider healthFill;
        [SerializeField] private Slider staminaFill;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI characterName;
        DisposableBag subscriprions;

        private void Start()
        {
            healthFill.minValue = 0;
            staminaFill.minValue = 0;
        }

        public void Bind(PlayerProfilePresenter presenter)
        {
            subscriprions.Add(presenter.Health.Subscribe(_val =>
            {
                UpdateHealthBar(_val, presenter.MaxHealth.CurrentValue);
            }));
            subscriprions.Add(presenter.MaxHealth.Subscribe(_val =>
            {
                UpdateHealthBar(presenter.Health.CurrentValue, _val);
            }));



            subscriprions.Add(presenter.Stamina.Subscribe(_val =>
            {
                UpdateStaminaBar(_val, presenter.MaxStamina.CurrentValue);
            }));

            subscriprions.Add(presenter.MaxStamina.Subscribe(_val =>
            {
                UpdateStaminaBar(presenter.Stamina.CurrentValue, _val);
            })); 
            
            subscriprions.Add(presenter.Name.Subscribe(_val =>
            {
                UpdateCharacterName(_val);
            }));
            subscriprions.AddTo(this);
        }
        public void Unbind()
        {
            subscriprions.Dispose();
        }
        private void UpdateHealthBar(float value, float maxValue)
        {
            healthFill.maxValue = maxValue;
            healthFill.value = value;
            healthText.text = value.ToString()+" \\ " + maxValue.ToString();
        }
        private void UpdateStaminaBar(float value, float maxValue)
        {
            staminaFill.maxValue = maxValue;
            staminaFill.value = value;
        }
        private void UpdateCharacterName(string newName)
        {
             characterName.text = newName;
        }
    }
}
