using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace LOGIYGames
{
    public class PlayerHealthView : MonoBehaviour
    {
        [SerializeField]
        private UIDocument document;

        private ProgressBar healthFill;
        private Label healthText;

        DisposableBag subscriprions;

        private void Start()
        {
            var root = document.rootVisualElement;

            healthFill = root.Q<ProgressBar>("health-fill");
            healthText = root.Q<Label>("health-text");
            healthFill.lowValue = 0;
        }

        public void Bind(PlayerHealthPresenter presenter)
        {
            subscriprions.Add(presenter.MaxHealth.Subscribe(_val =>
            {
                UpdateBar(_val, presenter.MaxHealth.CurrentValue);
            }));

            subscriprions.Add(presenter.Health.Subscribe(_val =>
            {
                UpdateBar(_val, presenter.MaxHealth.CurrentValue);
                UpdateText(_val);
            }));
            subscriprions.AddTo(this);
        }
        public void Unbind()
        {
            subscriprions.Dispose();
        }
        private void UpdateBar(float value, float maxValue)
        {
            healthFill.highValue = maxValue;
            healthFill.value = value;
        }
        private void UpdateText(float value)
        {
            healthText.text =
                $"{value}";
        }
    }
}
