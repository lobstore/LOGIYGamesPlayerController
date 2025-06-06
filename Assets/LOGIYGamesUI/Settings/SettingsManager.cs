using UnityEngine;
using UnityEngine.Audio;
namespace LOGIYGames
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        public string[] qualityPresetsNames { get; private set; }
        public float defaultRenderScale { get; private set; } = 1.0f;

        [field: SerializeField] public AudioMixer audioMixer { get; private set; }
        public string masterVolumeParam { get; private set; } = "MasterVolume";
        public string musicVolumeParam { get; private set; } = "MusicVolume";
        public string sfxVolumeParam { get; private set; } = "SFXVolume";

        public Resolution defaultResolution { get; private set; }
        public Resolution[] resolutions { get; private set; }

        public SettingsModel SettingsModel { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SettingsModel = new();

                defaultResolution = Screen.currentResolution;
                qualityPresetsNames = QualitySettings.names;
                resolutions = Screen.resolutions;
            }
            else
            {
                Destroy(gameObject);
            }
            // Инициализация разрешений

        }

        #region Graphics Settings
        public void SetQualityPreset(int presetIndex)
        {
            if (presetIndex >= 0 && presetIndex < qualityPresetsNames.Length)
            {
                QualitySettings.SetQualityLevel(presetIndex, true);
            }
            else
            {
                Debug.LogWarning($"Invalid quality preset index: {presetIndex}");
            }
        }
        public void SetVerticalSync(int vsyncCount)
        {
            QualitySettings.vSyncCount = vsyncCount;
        }
        public void SetAnisotropicFiltration(bool isAniso)
        {

            QualitySettings.anisotropicFiltering = isAniso ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;

        }
        public void SetAntiAliasing(int aaLevel)
        {
            // 0 = Disabled, 1 = 2x, 2 = 4x, 3 = 8x
            QualitySettings.antiAliasing = aaLevel;
        }

        public void SetShadowQuality(ShadowQuality quality)
        {
            QualitySettings.shadows = quality;
        }

        public void SetShadowResolution(ShadowResolution resolution)
        {
            QualitySettings.shadowResolution = resolution;
        }

        public void SetRenderScale(float scale)
        {
            scale = Mathf.Clamp(scale, 0.5f, 2.0f);
            QualitySettings.resolutionScalingFixedDPIFactor = scale;
        }

        public void SetVSyncCount(int count)
        {
            QualitySettings.vSyncCount = count;
        }

        public void SetTextureQuality(int qualityLevel)
        {
            QualitySettings.globalTextureMipmapLimit = qualityLevel;
        }

        public void SetDrawDistance(float distance)
        {
            // Это нужно применять к конкретным камерам или настройкам рендера
            // Пример для основной камеры
            Camera.main.farClipPlane = distance;
        }
        #endregion

        #region Display Settings
        public void SetResolution(int width, int height)
        {
            Screen.SetResolution(width, height, SettingsModel.isFullscreen);
        }

        public void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
        }

        public void SetBrightness(float brightness)
        {
            // Реализация зависит от платформы и может потребовать дополнительных плагинов
            // Это примерная реализация, которая может работать не на всех платформах
            SettingsModel.brightness = Mathf.Clamp(brightness, 0.1f, 1.0f);
            // Здесь должна быть логика применения яркости
        }
        #endregion

        #region Audio Settings
        public void SetMasterVolume(float volume)
        {
            SettingsModel.masterVolume = volume;
            SetAudioVolume(masterVolumeParam, SettingsModel.masterVolume);
        }

        public void SetMusicVolume(float volume)
        {
            SettingsModel.musicVolume = volume;
            SetAudioVolume(musicVolumeParam, SettingsModel.musicVolume);
        }

        public void SetSFXVolume(float volume)
        {
            SettingsModel.sfxVolume = volume;
            SetAudioVolume(sfxVolumeParam, SettingsModel.sfxVolume);
        }

        private void SetAudioVolume(string parameterName, float volume)
        {
            if (audioMixer != null)
            {
                // Конвертируем линейное значение 0-1 в логарифмическое значение в дБ
                float dB = ConvertLinearToDb(volume);
                audioMixer.SetFloat(parameterName, dB);
            }
        }
        #endregion


        private float ConvertLinearToDb(float linearValue)
        {
            // Обрабатываем тишину (0 или отрицательные значения)
            if (linearValue <= 0.0001f) // Практически ноль
            {
                return -80f; // Стандартный порог "тишины" в Unity
            }

            // Основная формула преобразования
            return Mathf.Log10(linearValue) * 20f;
        }
        #region Gameplay Settings
        public void SetMouseSensitivity(float sensitivity)
        {
            SettingsModel.sensitivity = Mathf.Clamp(sensitivity, 0.1f, 10f);
        }

        public float GetMouseSensitivity()
        {
            return SettingsModel.sensitivity;
        }

        public void SetInvertMouseY(bool invert)
        {
            // Сохраняем в PlayerPrefs или другом хранилище настроек
            PlayerPrefs.SetInt("InvertMouseY", invert ? 1 : 0);
        }
        #endregion

        #region Save/Load Settings
        public void SaveSettings()
        {
            PlayerPrefs.SetInt("QualityLevel", SettingsModel.qualityLevel);
            PlayerPrefs.SetFloat("RenderScale", QualitySettings.resolutionScalingFixedDPIFactor);
            PlayerPrefs.SetInt("ResolutionIndex", SettingsModel.resolutionIndex);
            PlayerPrefs.SetString("Fullscreen", SettingsModel.isFullscreen.ToString());
            PlayerPrefs.SetFloat("Brightness", SettingsModel.brightness);
            PlayerPrefs.SetFloat("MouseSensitivity", SettingsModel.sensitivity);
            PlayerPrefs.SetFloat(masterVolumeParam, SettingsModel.masterVolume);
            PlayerPrefs.SetFloat(musicVolumeParam, SettingsModel.musicVolume);
            PlayerPrefs.SetFloat(sfxVolumeParam, SettingsModel.sfxVolume);

            PlayerPrefs.Save();
        }

        public void LoadSettings()
        {
            // Графика
            SetQualityPreset(PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel()));
            SetRenderScale(PlayerPrefs.GetFloat("RenderScale", defaultRenderScale));

            // Дисплей
            int width = PlayerPrefs.GetInt("ResolutionWidth", defaultResolution.width);
            int height = PlayerPrefs.GetInt("ResolutionHeight", defaultResolution.height);
            bool fullscreen = bool.Parse(PlayerPrefs.GetString("Fullscreen", "true"));
            SetResolution(width, height);
            SetBrightness(PlayerPrefs.GetFloat("Brightness", 1.0f));

            // Громкость
            if (audioMixer != null)
            {
                SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1.0f));
                SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1.0f));
                SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1.0f));
            }

            // Геймплей
            SetMouseSensitivity(PlayerPrefs.GetFloat("MouseSensitivity", 1.0f));
        }
        #endregion
    }
}