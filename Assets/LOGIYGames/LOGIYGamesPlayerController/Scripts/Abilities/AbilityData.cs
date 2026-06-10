using LOGIYGames.Shared.Character.Events;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    [CreateAssetMenu( fileName = "New Ability Data", menuName = "Ability/AbilityData")]
    public class AbilityData : ScriptableObject
    {
        [Header("Info")]
        public string label;
        public Sprite icon;

        [Header("Timeline")]

        public string castingAnimation;
        public float castDuration = 1f;
        public float cooldown;

        public float executionDuration = 1f;

        [Header("Animations")]
        public List<AnimationTimedEvent> Animations;


        [Header("Targetings")]
        public List<TargetingTimedEvent> TargetingFactories;

    }
}
