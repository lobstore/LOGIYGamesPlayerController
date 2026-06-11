using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class WeaponController :MonoBehaviour
    {
        public WeaponDataSO CurrentWeapon
        {
            get;
            private set;
        }

        private CharacterModule character;

        private Animator animator;

        private void Awake()
        {
            character = GetComponent<CharacterModule>();
            animator = GetComponent<Animator>();
        }

        // ========================================================
        // EQUIP
        // ========================================================

        public void EquipWeapon(
            WeaponDataSO data)
        {
            CurrentWeapon = data;

            if (CurrentWeapon
                    .AnimatorOverride
                != null)
            {
                animator.runtimeAnimatorController =
                    CurrentWeapon
                        .AnimatorOverride;
            }
        }

        // ========================================================
        // GET MOVESET
        // ========================================================

        public ComboMovesetSO GetWeaponCombo()
        {
            if (CurrentWeapon == null)
                return null;

            return CurrentWeapon.ComboSet;
        }
    }
}
