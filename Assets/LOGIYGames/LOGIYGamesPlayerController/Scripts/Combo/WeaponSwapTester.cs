using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class WeaponSwapTester : MonoBehaviour
    {
        [SerializeField]
        private Character character;

        [SerializeField]
        private WeaponDataSO sword;

        [SerializeField]
        private WeaponDataSO spear;

        [SerializeField]
        private WeaponDataSO greatsword;

        [SerializeField]
        private WeaponDataSO defaultWeapon;

        private void Start()
        {
            character
                    .WeaponController
                    .EquipWeapon(defaultWeapon);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                character
                    .WeaponController
                    .EquipWeapon(sword);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                character
                    .WeaponController
                    .EquipWeapon(spear);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                character
                    .WeaponController
                    .EquipWeapon(greatsword);
            }
        }
    }
}