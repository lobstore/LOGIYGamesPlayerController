using System;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "New EffectData", menuName = "Abilities/Effects/EffectData")]
    public class EffectData : ScriptableObject
    {
        public GameObject VFX;
        public AudioClip SFX;
        public Sprite Icon;
    }
}
