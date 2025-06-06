using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace LOGIYGames
{
    public class SettingsPopup : PopupBaseMono
    {
        [SerializeField] private Transform _settingsContainer;
        [SerializeField] Button applyButton;
        private SettingsManager settingsManager;
        [SerializeField] TMP_Dropdown GrapicsPresetsDropdown;
        [SerializeField] TMP_Dropdown ScreenDropdown;
        [SerializeField] Slider MasterVolume;
        private void Awake()
        {
            settingsManager = SettingsManager.Instance;
            applyButton.onClick.AddListener(ApplySettings);
            settingsManager.LoadSettings();
        }
        protected override void Start()
        {
            base.Start();
            Initialize();
        }
        public override void Hide()
        {
            base.Hide();
        }

        public override void Show()
        {
            base.Show();

        }

        public void ApplySettings()
        {
            settingsManager.SaveSettings();
        }

        private void Initialize()
        {
            GrapicsPresetsDropdown.ClearOptions();
            GrapicsPresetsDropdown.AddOptions(settingsManager.qualityPresetsNames.ToList());
            GrapicsPresetsDropdown.value = QualitySettings.GetQualityLevel();

            MasterVolume.maxValue = 1f;
            MasterVolume.value = settingsManager.SettingsModel.masterVolume;

            ScreenDropdown.ClearOptions();
            var resolutions = settingsManager.resolutions;
            List<string> options = new List<string>();
            int curRes = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    curRes = i;
                }
            }
            ScreenDropdown.AddOptions(options);
            ScreenDropdown.value = curRes;
            ScreenDropdown.RefreshShownValue();
        }

        public void SetResolution(int index)
        {
            Resolution resolution = settingsManager.resolutions[index];
            settingsManager.SetResolution(resolution.width, resolution.height);
        }

        public void SetQuality(int index)
        {
            settingsManager.SetQualityPreset(index);
        }

        public void SetMasterVolume(float value)
        {
            settingsManager.SetMasterVolume(value);
        }
        public void SetSFXVolume(float value)
        {
            settingsManager.SetSFXVolume(value);
        }

        private void OnDestroy()
        {
            applyButton.onClick.RemoveListener(ApplySettings);
        }
    }
}