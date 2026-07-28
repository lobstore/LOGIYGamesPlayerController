using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LOGIYGames
{
    public class PlayerEffectView : MonoBehaviour
    {
        [SerializeField] Image effectIcon;
        [SerializeField] TextMeshProUGUI displayText;
        DisposableBag subscriprions;
        public void Bind(PlayerEffectPresenter presenter)
        {
            subscriprions.Add(presenter.DisplayValue.Subscribe(
                (value) =>
                {
                    UpdateEffectDisplayText(value);
                }
                ));

            subscriprions.Add(presenter.Icon.Subscribe((sprite) =>
            {
                UpdateEffectIcon(sprite);
            }));
            subscriprions.AddTo(this);
        }

        private void UpdateEffectDisplayText(string value)
        {
            displayText.text = value;
        }

        private void UpdateEffectIcon(Sprite skillIcon)
        {
            this.effectIcon.sprite = skillIcon;
        }
        public void Unbind()
        {
            subscriprions.Dispose();
        }

    }
}
