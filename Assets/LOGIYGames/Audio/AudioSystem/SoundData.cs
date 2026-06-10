using System;
using UnityEngine;
using UnityEngine.Audio;

namespace LOGIYGames.Audio
{
    [Serializable]
    public class SoundData
    {
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        public bool loop;
        public bool playOnAwake;
        public bool frequentSound;

        public bool mute;
        public bool bypassEffects;
        public bool bypassListenerEffects;
        public bool bypassReverbZones;

        public int priority = 128;
        [Range(0,1)]
        public float volume = 1f;
        [Range(-3,3)]
        public float pitch = 1f;

        public float minPitchDelta = -0.05f;
        public float maxPitchDelta = 0.05f;
        [Range(-1,1)]
        public float panStereo;
        [Range(0,1)]
        public float spatialBlend;
        [Range(0,1.1f)]
        public float reverbZoneMix = 1f;
        [Range(0,5)]
        public float dopplerLevel = 1f;
        [Range(0,360)]
        public float spread;
        [Min(0)]
        public float minDistance = 1f;
        [Min(0)]
        public float maxDistance = 500f;

        public bool ignoreListenerVolume;
        public bool ignoreListenerPause;

        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    }
}