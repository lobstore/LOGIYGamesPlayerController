using LOGIYGames.CharacterCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace LOGIYGames
{
    /// <summary>
    /// Module for managing character sound effects at specific attachment points.
    /// Creates AudioSources at specified transforms and plays sounds through them.
    /// Supports animation events for timing-based audio playback.
    /// </summary>
    public class AudioModule : MonoBehaviour
    {
        [Serializable]
        public class AudioPoint
        {
            [Tooltip("Name of this audio point (e.g., 'Foot_L', 'Hand_R', 'Head')")]
            public string pointName;
            
            [Tooltip("Transform to attach the AudioSource to. If null, uses the main transform.")]
            public Transform attachTransform;
            
            [Tooltip("Default AudioClip to play from this point")]
            public AudioClip defaultClip;
            
            [Tooltip("Volume for this audio point")]
            [Range(0f, 1f)]
            public float volume = 1f;
            
            [Tooltip("Pitch variation range for variety")]
            [Range(0f, 1f)]
            public float pitchVariation = 0f;
            
            [Tooltip("Whether this source should loop")]
            public bool loop = false;
            
            [Tooltip("Spatial blend (0 = 2D, 1 = 3D)")]
            [Range(0f, 1f)]
            public float spatialBlend = 0f;
            
            [Tooltip("Min distance for 3D sound falloff")]
            public float minDistance = 1f;
            
            [Tooltip("Max distance for 3D sound falloff")]
            public float maxDistance = 100f;
        }

        [Header("Audio Points Configuration")]
        [Tooltip("List of audio attachment points on the character")]
        [SerializeField]
        private List<AudioPoint> _audioPoints = new List<AudioPoint>();

        [Header("Global Settings")]
        [Tooltip("Master volume multiplier for all audio points")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _masterVolume = 1f;

        [Tooltip("AudioMixerGroup for all character sounds (optional)")]
        [SerializeField]
        private AudioMixerGroup _audioMixerGroup;

        // Dictionary mapping point names to their AudioSources
        private Dictionary<string, AudioSource> _audioSourceMap;
        
        // Dictionary mapping point names to their AudioPoint configurations
        private Dictionary<string, AudioPoint> _audioPointMap;

        /// <summary>
        /// Gets an AudioSource by point name
        /// </summary>
        public AudioSource GetAudioSource(string pointName)
        {
            if (_audioSourceMap.TryGetValue(pointName, out AudioSource source))
            {
                return source;
            }
            Debug.LogWarning($"Audio point '{pointName}' not found!");
            return null;
        }

        /// <summary>
        /// Plays a sound at the specified audio point
        /// </summary>
        /// <param name="pointName">Name of the audio point</param>
        /// <param name="clip">Clip to play (optional, uses default if null)</param>
        /// <param name="volumeOverride">Override volume (optional)</param>
        public void PlaySoundAtPoint(string pointName, AudioClip clip = null, float? volumeOverride = null)
        {
            if (!_audioSourceMap.TryGetValue(pointName, out AudioSource source))
            {
                Debug.LogWarning($"Audio point '{pointName}' not found!");
                return;
            }

            if (!_audioPointMap.TryGetValue(pointName, out AudioPoint point))
            {
                Debug.LogWarning($"Audio point configuration '{pointName}' not found!");
                return;
            }

            AudioClip clipToPlay = clip ?? point.defaultClip;
            if (clipToPlay == null)
            {
                Debug.LogWarning($"No AudioClip specified for audio point '{pointName}'!");
                return;
            }

            source.clip = clipToPlay;
            source.volume = (volumeOverride ?? point.volume) * _masterVolume;
            source.pitch = 1f + UnityEngine.Random.Range(-point.pitchVariation, point.pitchVariation);
            source.loop = point.loop;
            source.Play();
        }

        /// <summary>
        /// Plays a sound at the specified audio point with one-shot (interruptible)
        /// </summary>
        public void PlayOneShotAtPoint(string pointName, AudioClip clip = null, float volumeScale = 1f)
        {
            if (!_audioSourceMap.TryGetValue(pointName, out AudioSource source))
            {
                Debug.LogWarning($"Audio point '{pointName}' not found!");
                return;
            }

            AudioClip clipToPlay = clip ?? (_audioPointMap.TryGetValue(pointName, out AudioPoint point) ? point.defaultClip : null);
            if (clipToPlay == null)
            {
                Debug.LogWarning($"No AudioClip specified for audio point '{pointName}'!");
                return;
            }

            source.PlayOneShot(clipToPlay, volumeScale * _masterVolume);
        }

        /// <summary>
        /// Stops sound at the specified audio point
        /// </summary>
        public void StopSoundAtPoint(string pointName)
        {
            if (_audioSourceMap.TryGetValue(pointName, out AudioSource source))
            {
                source.Stop();
            }
        }

        /// <summary>
        /// Stops all sounds at all audio points
        /// </summary>
        public void StopAllSounds()
        {
            foreach (var source in _audioSourceMap.Values)
            {
                source.Stop();
            }
        }

        /// <summary>
        /// Pauses/Resumes sound at the specified audio point
        /// </summary>
        public void PauseSoundAtPoint(string pointName, bool pause)
        {
            if (_audioSourceMap.TryGetValue(pointName, out AudioSource source))
            {
                source.Pause();
            }
        }

        /// <summary>
        /// Sets the volume of a specific audio point
        /// </summary>
        public void SetPointVolume(string pointName, float volume)
        {
            if (_audioPointMap.TryGetValue(pointName, out AudioPoint point))
            {
                point.volume = Mathf.Clamp01(volume);
            }
            
            if (_audioSourceMap.TryGetValue(pointName, out AudioSource source))
            {
                source.volume = point.volume * _masterVolume;
            }
        }

        /// <summary>
        /// Sets the master volume for all audio points
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
        }

        /// <summary>
        /// Animation event callback for playing sounds
        /// Call this from animation events with the point name and optional clip name
        /// </summary>
        public void PlaySoundEvent(string pointName)
        {
            PlaySoundAtPoint(pointName);
        }

        /// <summary>
        /// Animation event callback for playing one-shot sounds
        /// </summary>
        public void PlayOneShotEvent(string pointName)
        {
            PlayOneShotAtPoint(pointName);
        }

        /// <summary>
        /// Animation event callback for stopping sounds
        /// </summary>
        public void StopSoundEvent(string pointName)
        {
            StopSoundAtPoint(pointName);
        }

        void Start()
        {
            InitializeAudioPoints();
        }

        /// <summary>
        /// Initializes all audio points and creates AudioSources
        /// </summary>
        private void InitializeAudioPoints()
        {
            _audioSourceMap = new Dictionary<string, AudioSource>();
            _audioPointMap = new Dictionary<string, AudioPoint>();

            foreach (var point in _audioPoints)
            {
                if (string.IsNullOrEmpty(point.pointName))
                {
                    Debug.LogWarning("Found audio point with empty name, skipping...");
                    continue;
                }

                // Check for duplicate names
                if (_audioPointMap.ContainsKey(point.pointName))
                {
                    Debug.LogWarning($"Duplicate audio point name '{point.pointName}', skipping...");
                    continue;
                }

                // Get the transform to attach to
                Transform targetTransform = point.attachTransform ?? transform;

                // Create AudioSource on the target transform
                AudioSource source = targetTransform.GetComponent<AudioSource>();
                if (source == null)
                {
                    source = targetTransform.gameObject.AddComponent<AudioSource>();
                }

                // Configure the AudioSource
                source.playOnAwake = false;
                source.spatialBlend = point.spatialBlend;
                source.minDistance = point.minDistance;
                source.maxDistance = point.maxDistance;
                source.loop = point.loop;
                source.volume = point.volume * _masterVolume;
                source.pitch = 1f;
                
                if (_audioMixerGroup != null)
                {
                    source.outputAudioMixerGroup = _audioMixerGroup;
                }

                // Store references
                _audioSourceMap[point.pointName] = source;
                _audioPointMap[point.pointName] = point;

                // Set default clip if specified
                if (point.defaultClip != null)
                {
                    source.clip = point.defaultClip;
                }
            }

        }

        private void OnDestroy()
        {
            // Clean up dynamically created AudioSources if needed
            _audioSourceMap?.Clear();
            _audioPointMap?.Clear();
        }
    }
}
