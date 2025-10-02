namespace LOGIYGames
{
    [System.Serializable]
    public class SettingsModel
    {
        public float masterVolume;
        public float sfxVolume;
        public float musicVolume;
        public float sensitivity;
        public bool isMotionBlurOn;
        public int qualityLevel;
        public int resolutionIndex;
        public int shadowsResolutionIndex;
        public int antialiasLevel;

        public bool isFullscreen;
        public float brightness;
    }
}