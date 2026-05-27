using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class WeaponController
    {
        public WeaponDataSO CurrentWeapon
        {
            get;
            private set;
        }

        private readonly CharacterModule
            character;

        private readonly Animator
            animator;

        public WeaponController(
            CharacterModule owner)
        {
            character = owner;

            animator =
                owner.GetComponent<Animator>();
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
