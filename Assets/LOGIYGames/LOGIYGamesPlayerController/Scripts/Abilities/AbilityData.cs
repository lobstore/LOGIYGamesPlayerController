using LOGIYGames.Shared.Character.Events;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(
        fileName = "New Ability Data",
        menuName = "Ability/AbilityData")]
    public class AbilityData : ScriptableObject
    {
        [Header("Info")]
        public string label;

        [Header("Timeline")]
        public string castingAnimation;
        public float castDuration = 1f;

        public float executionDuration = 1f;
        public float targetingStartTime;

        [Header("Animations")]
        public List<AnimationTimedEvent> Animations;


        [Header("Targetings")]
        public List<TargetingTimedEvent> TargetingFactories;

    }
}
