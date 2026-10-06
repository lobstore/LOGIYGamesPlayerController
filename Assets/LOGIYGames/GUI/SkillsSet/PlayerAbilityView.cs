using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LOGIYGames
{
    public class PlayerAbilityView : MonoBehaviour
    {
        [SerializeField] Image skillIcon;
        [SerializeField] Image cooldownFill;
        [SerializeField] TextMeshProUGUI cooldownTimeText;
        DisposableBag subscriprions;

        public void Bind(PlayerAbilityPresenter presenter)
        {
            subscriprions.Add(presenter.CooldownProgress.Subscribe(
                (currentTime) =>
                {
                    UpdateSkillCooldownFill(currentTime);
                }
                ));
            subscriprions.Add(presenter.CooldownTime.Subscribe(
                (currentTime) =>
                {
                    UpdateSkillCooldownText(currentTime);
                }
                ));
            subscriprions.Add(presenter.Icon.Subscribe((sprite) =>
            {
                UpdateSkillIcon(sprite);
            }));
            subscriprions.AddTo(this);
        }
        public void Unbind()
        {
            subscriprions.Dispose();
        }
        private void UpdateSkillCooldownFill(float value)
        {
            cooldownFill.fillAmount = Mathf.Clamp01(value);
        }
        private void UpdateSkillIcon(Sprite skillIcon)
        {
            this.skillIcon.sprite = skillIcon;
        }
        private void UpdateSkillCooldownText(float skillCooldown)
        {
            int rounded = Mathf.RoundToInt(skillCooldown);
            cooldownTimeText.text = rounded.ToString();
            if (cooldownFill.fillAmount <= 0)
            {
                cooldownTimeText.enabled = false;
            }
            else
            {
                cooldownTimeText.enabled = true;
            }
        }
    }
}
