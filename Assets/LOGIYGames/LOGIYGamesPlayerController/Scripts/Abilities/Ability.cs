using LOGIYGames.Shared.Enums;
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
        public string executionAnimation;

        // ========================================================
        // EFFECTS
        // ========================================================

        [Header("Effects")]
        public List<EffectFactory> effects =
            new();

    }
}
