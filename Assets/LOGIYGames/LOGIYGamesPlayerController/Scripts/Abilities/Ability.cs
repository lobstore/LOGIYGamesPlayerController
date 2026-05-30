using LOGIYGames.Shared.Character.Events;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(
        fileName = "New Ability",
        menuName = "Ability/Ability")]
    public class Ability : ScriptableObject
    {
        [Header("Info")]
        public string label;

        // ========================================================
        // CAST
        // ========================================================

        [Header("Casting")]
        public string castingAnimation;
        public float castDuration = 1f;

        // ========================================================
        // EXECUTION
        // ========================================================

        [Header("Execution")]

        public float executionDuration = 1f;
        // ========================================================
        // TIMELINE
        // ========================================================

        [Header("Timeline")]
        public List<AbilityTimedEvent> TimedEvents = new();

        // ========================================================
        // EFFECTS
        // ========================================================

        [Header("Legacy Effects")]
        public List<EffectFactory> Effects = new();
    }

}
