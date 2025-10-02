using UnityEngine;
using UnityEngine.UIElements;
namespace LOGIYGames
{
    public class SettingView : MonoBehaviour
    {
        public virtual void Set(SettingsManager settingsManager)
        {

        }
    }
    public class SliderSettingView : SettingView
    {
        [SerializeField] Slider Slider;

        override public void Set(SettingsManager settingsManager)
        {

        }

        public void UpdateView()
        {

        }
    }
}