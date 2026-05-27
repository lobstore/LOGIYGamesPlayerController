using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]
    public class Ability : ScriptableObject
    {
        public string label;

        public List<EffectFactory> effects = new();

        public string executionAnimationName;
        public string castingAnimationName;

        public float castDuration;
        public float cooldown;

    }
}
