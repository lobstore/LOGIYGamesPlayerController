using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace LOGIYGames
{
    public class TooltipManager : MonoBehaviour
    {
        public static TooltipManager Instance;

        [SerializeField] private GameObject tooltipObject;
        [SerializeField] private TextMeshProUGUI tooltipText;
        [SerializeField] private RectTransform canvasRectTransform;
        [SerializeField] private CanvasGroup canvasGroup;
        DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> tween;
        private void Awake()
        {
            Instance = this;
            HideTooltip();
        }

        private void Update()
        {
            if (tooltipObject.activeSelf)
            {

            }
        }

        public void ShowTooltip(string text)
        {
            tooltipText.text = text;
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, null, out position);
            tooltipObject.GetComponent<RectTransform>().localPosition = position;
            tooltipObject.SetActive(true);
             tween = DOTween.To(()=> canvasGroup.alpha, x=> canvasGroup.alpha = x, 1, 0.5f).SetDelay(1f);
        }
        public void HideTooltip()
        {
            tween.Kill();
            tween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, 0.5f);
        }
    }
}
