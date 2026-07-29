using LOGIYGames.Shared.Enums;
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

        public AnimationData Animation;

        // =====================================================
        // MOVEMENT
        // =====================================================

        [Header("Movement")]

        public float ForwardImpulse;

        // =====================================================
        // DAMAGE
        // =====================================================

        [Header("Damage")]

        DamageData BaseDamageData;

        // =====================================================
        // SEQUENCE
        // =====================================================

        [Header("Transitions")]

        public List<AttackTransition>
            Transitions = new();
    }
}
