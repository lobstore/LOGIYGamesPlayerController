using LOGIYGames.Shared.Enums;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    [CreateAssetMenu(
        menuName = "Combat/Weapon")]
    public class WeaponDataSO
        : ScriptableObject
    {
        [Header("Info")]

        public string WeaponName;

        public WeaponType WeaponType;

        // =====================================================
        // MOVESET
        // =====================================================

        [Header("Moveset")]

        public ComboMovesetSO Moveset;

        // =====================================================
        // ANIMATIONS
        // =====================================================

        [Header("Animations")]

        public AnimatorOverrideController
            AnimatorOverride;

        // =====================================================
        // STATS
        // =====================================================

        [Header("Stats")]

        public int BaseDamage = 10;

        public float StaminaMultiplier = 1f;
    }
}
