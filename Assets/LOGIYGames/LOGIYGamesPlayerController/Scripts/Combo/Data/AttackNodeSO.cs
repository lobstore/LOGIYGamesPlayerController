using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    [CreateAssetMenu(
        menuName = "Combat/Attack Node")]
    public class AttackNodeSO
        : ScriptableObject
    {
        // =====================================================
        // ANIMATION
        // =====================================================

        [Header("Animation")]

        public string AnimationStateName;

        public float CrossFade = 0.1f;

        public bool UseRootMotion = true;

        // =====================================================
        // TIMING
        // =====================================================

        [Header("Timing")]

        public float Duration = 1f;

        public float ComboWindowStart = 0.25f;

        public float ComboWindowEnd = 0.7f;

        public float TransitionTime = 0.85f;

        public float CancelTime = 0.6f;

        // =====================================================
        // MOVEMENT
        // =====================================================

        [Header("Movement")]

        public float ForwardImpulse;

        // =====================================================
        // DAMAGE
        // =====================================================

        [Header("Damage")]

        public int Damage = 10;

        public float PoiseDamage = 10f;

        // =====================================================
        // ACTIONS
        // =====================================================

        [Header("Transitions")]

        public List<AttackTransition>
            Transitions = new();
    }
}
