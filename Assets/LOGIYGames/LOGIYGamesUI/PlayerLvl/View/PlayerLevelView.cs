using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class PlayerLevelView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentLvl;
        [SerializeField] private TextMeshProUGUI nextLvl;
        [SerializeField] private Slider _xpSlider;
        [SerializeField] private Slider _xpSliderDelta;

        private Coroutine xpCoroutine;
        int prevLvl = 0;
        float prevXp = 0;

        int _currentlvl;
        int _currentxp;
        int _requirexp;
        public void Initialize(PlayerLevelModel playerLevelModel)
        {
            prevLvl = playerLevelModel.Level;
            prevXp = playerLevelModel.CurrentXP;
            UpdateLvlViews(playerLevelModel.Level);
            UpdateView(playerLevelModel);
        }

        public void UpdateView(PlayerLevelModel playerLevelModel)
        {
            _currentlvl = playerLevelModel.Level;
            _currentxp = playerLevelModel.CurrentXP;
            _requirexp = playerLevelModel.RequireXP;
            if (xpCoroutine != null)
            {
                StopCoroutine(xpCoroutine);

            }
            xpCoroutine = StartCoroutine(IncrementXP());


        }
        IEnumerator IncrementXP()
        {
            _xpSlider.maxValue = _requirexp;
            _xpSliderDelta.maxValue = _requirexp;
            _xpSlider.value = prevXp;
            _xpSliderDelta.value = prevXp;
            var lvl = _currentlvl;
            float startXP = prevXp; // Начальное значение
            float duration = 2f;    // Длительность анимации
            bool isNextLvl = false;
            float currentValue;
            float endValue = _currentxp;
            float elapsedTime = 0f;
            if (prevLvl < lvl)
            {
                endValue = _xpSlider.maxValue;
                isNextLvl = true;
            }
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                currentValue = Mathf.Lerp(startXP, endValue, elapsedTime / duration);
                _xpSliderDelta.value = currentValue;
                yield return null; // Ждем один кадр
            }
            if (isNextLvl)
            {
                _xpSlider.value = 0;
                UpdateLvlViews(lvl);
                endValue = _currentxp;
                elapsedTime = 0f;
                startXP = 0;
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    currentValue = Mathf.Lerp(startXP, endValue, elapsedTime / duration);
                    _xpSliderDelta.value = currentValue;
                    yield return null; // Ждем один кадр
                }
            }

            _xpSliderDelta.value = _currentxp;
            prevXp = _currentxp;
            prevLvl = lvl;
            xpCoroutine = null;
        }
        void UpdateLvlViews(int curlvl)
        {
            currentLvl.text = curlvl.ToString();
            nextLvl.text = (curlvl + 1).ToString();
        }
    }
}