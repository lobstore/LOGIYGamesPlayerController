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

        public void Bind(PlayerSkillPresenter presenter)
        {
            subscriprions.Add(presenter.Cooldown.Subscribe(
                (currentTime) =>
                {

                    UpdateSkillCooldownFill(currentTime);
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
            if (value <= 0.01f || value == 1)
            {
                cooldownFill.fillAmount = 0;
            }
        }
        private void UpdateSkillIcon(Sprite skillIcon)
        {
            this.skillIcon.sprite = skillIcon;
        }
        private void UpdateSkillCooldownText(float skillCooldown)
        {
            cooldownTimeText.text = skillCooldown.ToString();
            if (cooldownFill.fillAmount <=0)
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
