using LOGIYGames.Shared.Enums;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "New Ability Data", menuName = "Ability/AbilityData")]
    public class AbilityData: ScriptableObject
    {
        public Sprite Icon;

        public float CastDuration;
        public float ExecutionDuration;
        public float CooldownDuration;

        public AnimationData castingAnimation;

        [Header("Effects")]
        public List<EffectFactory> Effects = new();

        public TargetingFactory TargetingStrategy;
    }
}
