using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class SliderToTMPSubscriber : MonoBehaviour
    {
        [SerializeField] Slider Slider;
        TextMeshProUGUI TextMeshProUGUI;
        private void Awake()
        {
            TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
        }

        private void ChangeText(float value)
        {
            TextMeshProUGUI.text = value.ToString();
        }
        private void OnEnable()
        {
            if (Slider != null)
            {
                TextMeshProUGUI.text = Slider.value.ToString();
                Slider.onValueChanged.AddListener(ChangeText);
            }
        }
        private void OnDisable()
        {
            if (Slider != null)
            {
                Slider.onValueChanged.RemoveListener(ChangeText);
            }
        }
        private void OnDestroy()
        {
            if (Slider != null)
            {
                Slider.onValueChanged.RemoveListener(ChangeText);
            }
        }
    }
}