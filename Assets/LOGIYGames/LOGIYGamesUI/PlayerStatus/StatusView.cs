using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LOGIYGames
{
    public class StatusView : MonoBehaviour
    {
        private Coroutine healthChangeCoroutine;
        private Coroutine hungerChangeCoroutine;
        private Coroutine thirstChangeCoroutine;
        private Coroutine expirienceChangeCoroutine;
        [Header("Views")]
        [SerializeField] public Slider healthStatusSlider;
        [SerializeField] public Slider hungerStatusSlider;
        [SerializeField] public Slider thirstStatusSlider;
        [SerializeField] public Slider experienceStatusSlider;
        [SerializeField] float animationDuration;
        public IEnumerator AnimateValueChange(float value, Slider slider)
        {
            float startValue = slider.value;
            float elapsed = 0f;

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                slider.value = Mathf.Lerp(startValue, value, elapsed / animationDuration);
                yield return null;
            }

            slider.value = value;
        }
        public void OnHealthChanged(float value)
        {
            if (!gameObject.activeInHierarchy) return;
            if (healthChangeCoroutine != null)
            {
                StopCoroutine(healthChangeCoroutine);
            }
            healthChangeCoroutine = StartCoroutine(AnimateValueChange(value, healthStatusSlider));
        }

        public void OnHungerChanged(float value)
        {
            if (!gameObject.activeInHierarchy) return;
            if (hungerChangeCoroutine != null)
            {
                StopCoroutine(hungerChangeCoroutine);
            }
            hungerChangeCoroutine = StartCoroutine(AnimateValueChange(value, hungerStatusSlider));
        }


        public void OnThirstChanged(float value)
        {
            if (!gameObject.activeInHierarchy) return;
            if (thirstChangeCoroutine != null)
            {
                StopCoroutine(thirstChangeCoroutine);
            }
            thirstChangeCoroutine = StartCoroutine(AnimateValueChange(value, thirstStatusSlider));
        }

        public void OnExpirienceChanged(float value)
        {
            if (!gameObject.activeInHierarchy) return;
            if (expirienceChangeCoroutine != null)
            {
                StopCoroutine(expirienceChangeCoroutine);
            }
            expirienceChangeCoroutine = StartCoroutine(AnimateValueChange(value, experienceStatusSlider));
        }

    }
}
