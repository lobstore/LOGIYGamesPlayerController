using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class WeaponSwapTester : MonoBehaviour
    {
        [SerializeField]
        private CharacterModule character;

        [SerializeField]
        private WeaponDataSO sword;
        [SerializeField]
        private WeaponDataSO unarmed;
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
                    .EquipWeapon(unarmed);
            }
        }
    }
}