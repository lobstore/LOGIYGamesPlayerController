using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames
{
    public class WeaponSlot : MonoBehaviour
    {
        public WeaponSlotType weaponSlot;
        GameObject currentWeaponPrefab;

        public void UnloadModel()
        {
            if (currentWeaponPrefab!=null)
            {
                Destroy(currentWeaponPrefab);
            }
        }

        public void LoadModel(GameObject prefab)
        {
            currentWeaponPrefab = prefab;

            currentWeaponPrefab.transform.parent = transform;
            currentWeaponPrefab.transform.localPosition = Vector3.zero;
            currentWeaponPrefab.transform.localRotation = Quaternion.identity;
            //currentWeaponPrefab.transform.localScale = Vector3.one;


        }
    }
}
