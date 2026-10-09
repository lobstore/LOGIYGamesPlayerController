using LOGIYGames.CharacterCore;
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
        Health health;
        Stamina stamina;
        private void Start()
        {
            healthFill.minValue = 0;
            staminaFill.minValue = 0;
        }
        private void LateUpdate()
        {
            UpdateHealthBar(health.Current, health.Max);
            UpdateStaminaBar(stamina.Current, stamina.Max);
        }
        public void Bind(PlayerProfilePresenter presenter)
        {
            health = presenter.HealthModel;
            stamina = presenter.StaminaModel;

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
            healthText.text = value.ToString("0")+" \\ " + maxValue.ToString("0");
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
