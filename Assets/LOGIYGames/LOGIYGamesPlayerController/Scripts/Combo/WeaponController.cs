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

        private readonly Character
            _character;

        private readonly Animator
            _animator;

        public WeaponController(
            Character character)
        {
            _character = character;

            _animator =
                character.GetComponent<Animator>();
        }

        // =====================================================
        // EQUIP
        // =====================================================

        public void EquipWeapon(
            WeaponDataSO weapon)
        {
            if (weapon == null)
                return;

            CurrentWeapon = weapon;

            ApplyAnimatorOverride();
        }

        // =====================================================
        // HELPERS
        // =====================================================

        public ComboMovesetSO GetMoveset()
        {
            if (CurrentWeapon == null)
                return null;

            return CurrentWeapon.Moveset;
        }

        private void ApplyAnimatorOverride()
        {
            if (CurrentWeapon == null)
                return;

            if (CurrentWeapon.AnimatorOverride
                == null)
                return;

            _animator.runtimeAnimatorController =
                CurrentWeapon.AnimatorOverride;
        }
    }
}
