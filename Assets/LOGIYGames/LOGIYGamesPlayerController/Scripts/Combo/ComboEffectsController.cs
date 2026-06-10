using LOGIYGames.Audio;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LOGIYGames
{
    public class ComboEffectsController : MonoBehaviour
    {
        [SerializeField] Transform rHand;
        [SerializeField] Transform lHand;
        [SerializeField] Transform rFoot;
        [SerializeField] Transform lFoot;
        [SerializeField] Transform head;

        [SerializeField] List<SoundData> attackActionSounds;

        public void PlaySFX_RHand()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(rHand.position).WithRandomPitch(-0.1f, 0.1f).Play(attackActionSounds[Random.Range(0,attackActionSounds.Count)]);
        }
        public void PlaySFX_LHand()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(lHand.position).WithRandomPitch(-0.1f, 0.1f).Play(attackActionSounds[Random.Range(0, attackActionSounds.Count)]);
        }
        public void PlaySFX_RFoot()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(rFoot.position).WithRandomPitch(-0.1f,0.1f).Play(attackActionSounds[Random.Range(0, attackActionSounds.Count)]);
        }
        public void PlaySFX_LFoot()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(lFoot.position).WithRandomPitch(-0.1f, 0.1f).Play(attackActionSounds[Random.Range(0, attackActionSounds.Count)]);
        }
        public void PlaySFX_Head()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(rHand.position).WithRandomPitch(-0.1f, 0.1f).Play(attackActionSounds[Random.Range(0, attackActionSounds.Count)]);
        }
    }
}
