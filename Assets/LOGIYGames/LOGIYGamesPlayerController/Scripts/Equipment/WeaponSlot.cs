using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames
{
    public class WeaponSlot : MonoBehaviour
    {
        public WeaponSlotType weaponSlot;
        public GameObject CurrentWeaponPrefab {  get; private set; }
        public WeaponDataSO CurrentWeapon {  get; private set; }

        public void UnloadModel()
        {
            if (CurrentWeaponPrefab!=null)
            {
                Destroy(CurrentWeaponPrefab);
                CurrentWeapon = null;
            }
        }

        public void LoadModel(WeaponDataSO data)
        {
            CurrentWeapon = data;
            CurrentWeaponPrefab = Instantiate(data.Prefab);
            CurrentWeaponPrefab.transform.parent = transform;
            CurrentWeaponPrefab.transform.localPosition = Vector3.zero;
            CurrentWeaponPrefab.transform.localRotation = Quaternion.identity;
        }
    }
}
