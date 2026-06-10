using LOGIYGames.Audio;
using UnityEngine;

namespace LOGIYGames
{
    public class FootstepsController : MonoBehaviour
    {
        [SerializeField] Transform lFoot;
        [SerializeField] Transform rFoot;

        [SerializeField] SoundData soundData;
        public void PlayFootstepSoundR()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(rFoot.position).WithRandomPitch(-0.1f,0.1f).Play(soundData);
        }
        public void PlayFootstepSoundL()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(lFoot.position).WithRandomPitch(-0.1f, 0.1f).Play(soundData);
        }
    }
}
