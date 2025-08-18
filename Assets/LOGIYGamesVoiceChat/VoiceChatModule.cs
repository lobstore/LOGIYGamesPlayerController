using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
namespace LOGIYGames
{
    [RequireComponent(typeof(AudioSource))]
    public class VoiceChatModule : NetworkModuleBase
    {
        // Настройки
        [Header("Settings")]
        [SerializeField] private int sampleRate = 16000;
        [SerializeField] private float bufferDuration = 0.2f;
        [SerializeField] private float microphoneVolume = 1.0f;

        [SerializeField] InputReader inputReader;
        // Системные переменные
        private AudioClip microphoneClip;
        private AudioSource audioSource;
        private bool isTransmitting = false;
        private int lastSamplePos = 0;
        private List<float> audioBuffer = new List<float>();
        private List<byte> pendingAudio = new List<byte>();
        private bool isProcessing;

        [SerializeField] bool MuteForSelf = true;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
        }

        private void Start()
        {
            if (!IsLocalPlayer) return;

#if UNITY_WEBGL
        StartCoroutine(CheckMicrophonePermission());
#else
            StartMicrophone();
#endif
            inputReader.VoiceChatEvent.AddListener(ActivateVoiceChat);
            StopMicrophone();
        }
        void ActivateVoiceChat(InputAction.CallbackContext context)
        {
            if (!IsLocalPlayer) return;
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    StartTransmitting();
                    break;
                case InputActionPhase.Canceled:
                    StopTransmitting();
                    break;
                default:
                    break;
            }
        }
        private IEnumerator CheckMicrophonePermission()
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                StartMicrophone();
            }
            else
            {
                Debug.LogError("Microphone permission denied");
            }
        }

        private void StartMicrophone()
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("No microphones found");
                return;
            }

            microphoneClip = Microphone.Start(null, true, 1, sampleRate);
            StartCoroutine(WaitForMicrophoneReady());
        }
        private void StopMicrophone()
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("No microphones found");
                return;
            }

            Microphone.End(null);
        }
        private IEnumerator WaitForMicrophoneReady()
        {
            int attempts = 0;
            while (Microphone.GetPosition(null) <= 0 && attempts < 100)
            {
                attempts++;
                yield return new WaitForSeconds(0.01f);
            }

            if (attempts >= 100)
            {
                Debug.LogWarning("Microphone initialization failed");
            }
        }


        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (!isTransmitting || isProcessing) return;

            isProcessing = true;
            try
            {
                ProcessAudio();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            finally
            {
                isProcessing = false;
            }

        }
        private void StartTransmitting()
        {
            isTransmitting = true;
            StartMicrophone();
            lastSamplePos = Microphone.GetPosition(null);
        }

        private void StopTransmitting()
        {
            if (!isTransmitting) return;
            isTransmitting = false;

            if (pendingAudio.Count > 0)
            {
                SendAudioDataServerRpc(pendingAudio.ToArray());
                pendingAudio.Clear();

            }
        }

        private void ProcessAudio()
        {
            int currentSamplePos = Microphone.GetPosition(null);

            if (currentSamplePos < lastSamplePos)
            {
                ProcessAudioSegment(lastSamplePos, microphoneClip.samples - lastSamplePos);
                ProcessAudioSegment(0, currentSamplePos);
            }
            else if (currentSamplePos > lastSamplePos)
            {
                ProcessAudioSegment(lastSamplePos, currentSamplePos - lastSamplePos);
            }

            lastSamplePos = currentSamplePos;
        }

        private void ProcessAudioSegment(int offset, int sampleCount)
        {
            if (sampleCount <= 0) return;

            float[] samples = new float[sampleCount];
            microphoneClip.GetData(samples, offset);

            // Apply volume
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = Mathf.Clamp(samples[i] * microphoneVolume, -1f, 1f);
            }

            byte[] compressed = CompressAudio(samples);
            pendingAudio.AddRange(compressed);

            if (pendingAudio.Count >= sampleRate / 10) // ~100ms of data
            {
                Debug.Log("ProcessAudioSegment");
                SendAudioDataServerRpc(pendingAudio.ToArray());
                pendingAudio.Clear();
            }
        }

        [ServerRpc]
        private void SendAudioDataServerRpc(byte[] audioData)
        {
            if (audioData == null || audioData.Length == 0) return;

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = GetOtherClients()
                }
            };
            if (!IsHost)
                AddToAudioListener(audioData);
            BroadcastAudioDataClientRpc(audioData, clientRpcParams);
        }

        [ClientRpc]
        private void BroadcastAudioDataClientRpc(byte[] audioData, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner && MuteForSelf)
            {
                return;
            }
            AddToAudioListener(audioData);
        }

        private void AddToAudioListener(byte[] audioData)
        {
            float[] samples = DecompressAudio(audioData);
            AddToPlaybackBuffer(samples);
        }

        private ulong[] GetOtherClients()
        {
            List<ulong> clients = new List<ulong>();
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                clients.Add(clientId);
            }
            return clients.ToArray();
        }

        private byte[] CompressAudio(float[] samples)
        {
            byte[] data = new byte[samples.Length * 2];
            for (int i = 0; i < samples.Length; i++)
            {
                short value = (short)(samples[i] * short.MaxValue);
                data[i * 2] = (byte)(value & 0xFF);
                data[i * 2 + 1] = (byte)((value >> 8) & 0xFF);
            }
            return data;
        }

        private float[] DecompressAudio(byte[] data)
        {
            float[] samples = new float[data.Length / 2];
            for (int i = 0; i < samples.Length; i++)
            {
                short value = (short)((data[i * 2 + 1] << 8) | data[i * 2]);
                samples[i] = value / (float)short.MaxValue;
            }
            return samples;
        }

        private void AddToPlaybackBuffer(float[] samples)
        {
            audioBuffer.AddRange(samples);

            if (audioBuffer.Count >= sampleRate * bufferDuration && !audioSource.isPlaying)
            {
                PlayBufferedAudio();
            }
        }

        private void PlayBufferedAudio()
        {
            Debug.Log("audioBuffer.Count" + audioBuffer.Count);
            if (audioBuffer.Count == 0) return;

            AudioClip clip = AudioClip.Create("Voice", audioBuffer.Count, 1, sampleRate, false);
            clip.SetData(audioBuffer.ToArray(), 0);
            audioSource.clip = clip;
            audioSource.Play();

            // Очищаем буфер после воспроизведения
            audioBuffer.Clear();
        }

        public override void OnDestroy()
        {
            if (!IsLocalPlayer)
            {
                return;
            }
            if (Microphone.IsRecording(null))
            {
                Microphone.End(null);
            }


            inputReader.VoiceChatEvent.RemoveListener(ActivateVoiceChat);

        }
        public void SetMicrophoneVolume(float volume)
        {
            if (!IsLocalPlayer)
            {
                return;
            }
            microphoneVolume = Mathf.Clamp(volume, 0f, 2f);
        }
    }
}