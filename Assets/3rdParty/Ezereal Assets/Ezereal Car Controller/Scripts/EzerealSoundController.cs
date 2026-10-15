using UnityEngine;

namespace Ezereal
{
    public class EzerealSoundController : MonoBehaviour // This system plays tire and engine sounds.
    {
        [Header("References")]
        [SerializeField] bool useSounds = false;
        [SerializeField] EzerealCarController carController;
        [SerializeField] AudioSource tireAudio;
        [SerializeField] AudioSource engineAudio;

        [Header("Settings")]
        public float maxVolume = 0.5f; // Maximum volume for high speeds

        [Header("Debug")]
        [SerializeField] bool alreadyPlaying;

        void Start()
        {
            if (useSounds)
            {
                alreadyPlaying = false;

                if (carController == null || carController.vehicleRB == null || tireAudio == null || engineAudio == null)
                {
                    Debug.LogWarning("ezerealSoundController is missing some references. Ignore or attach them if you want to have sound controls.");


                }

                if (tireAudio != null)
                {
                    tireAudio.volume = 0f; // Start with zero volume
                    tireAudio.Stop();
                }

                carController.EngineStarted.AddListener(isstarted =>
                {
                    if (isstarted)
                    {
                        TurnOnEngineSound();
                    }
                    else
                    {
                        TurnOffEngineSound();
                    }
                });
            }
        }

        private void TurnOnEngineSound()
        {
            if (useSounds)
            {
                if (engineAudio != null)
                {
                    engineAudio.Play();
                }
            }
        }

        private void TurnOffEngineSound()
        {
            if (useSounds)
            {
                if (engineAudio != null)
                {
                    engineAudio.Stop();
                }
            }
        }

        void Update()
        {
            if (useSounds)
            {
#if UNITY_6000_0_OR_NEWER
                if (carController != null && carController.vehicleRB != null && tireAudio != null && engineAudio != null)
                {
                    if (carController.CurrentSpeed > 0 && (carController.CurrentBreakValue > 0 || carController.CurrentHandBreakValue > 0) && !carController.InAir())
                    {
                        tireAudio.Play();
                    }
                    else
                    {
                        tireAudio.Stop();
                    }

                    // Get the car's current speed
                    float speed = carController.vehicleRB.linearVelocity.magnitude;

                    // Calculate the volume based on speed
                    float targetVolume = Mathf.Clamp01(speed / 15) * maxVolume;


                    tireAudio.volume = targetVolume;

                    //Tire Pitch

                    float tireSoundPitch = 0.8f + (Mathf.Abs(carController.vehicleRB.linearVelocity.magnitude) / 50f);
                    tireAudio.pitch = tireSoundPitch;

                    //Engine Pitch

                    float engineSoundPitch = 0.8f + (Mathf.Abs(carController.vehicleRB.linearVelocity.magnitude) / 25f);
                    engineAudio.pitch = engineSoundPitch;
#else
            if (ezerealCarController != null && ezerealCarController.vehicleRB != null && tireAudio != null && engineAudio != null)
            {
                if (!ezerealCarController.stationary && !alreadyPlaying && !ezerealCarController.InAir())
                {
                    tireAudio.Play();
                    alreadyPlaying = true;
                }
                else if (ezerealCarController.stationary || ezerealCarController.InAir())
                {
                    tireAudio.Stop();
                    alreadyPlaying = false;
                }

                // Get the car's current speed
                float speed = ezerealCarController.vehicleRB.velocity.magnitude;

                // Calculate the volume based on speed
                float targetVolume = Mathf.Clamp01(speed / 15) * maxVolume;


                tireAudio.volume = targetVolume;

                //Tire Pitch

                float tireSoundPitch = 0.8f + (Mathf.Abs(ezerealCarController.vehicleRB.velocity.magnitude) / 50f);
                tireAudio.pitch = tireSoundPitch;

                //Engine Pitch

                float engineSoundPitch = 0.8f + (Mathf.Abs(ezerealCarController.vehicleRB.velocity.magnitude) / 25f);
                engineAudio.pitch = engineSoundPitch;
#endif
                }
            }
        }
    }
}
