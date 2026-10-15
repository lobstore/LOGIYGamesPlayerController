using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Enums;
using System;
using UnityEngine;

namespace LOGIYGames
{
    public class WeaponSlot : MonoBehaviour
    {
        public WeaponSlotType weaponSlot;
        public GameObject CurrentWeaponPrefab {  get; private set; }
        public WeaponController CurrentWeapon {  get; private set; }
        [SerializeField] GameObject Owner;
        public void UnloadModel()
        {
            if (CurrentWeaponPrefab!=null)
            {
                Destroy(CurrentWeaponPrefab);
                CurrentWeapon.SetOwner(null);
                CurrentWeapon = null;
            }
        }

        public void LoadModel(WeaponData data)
        {
            CurrentWeaponPrefab = Instantiate(data.Prefab);
            CurrentWeaponPrefab.transform.parent = transform;
            CurrentWeaponPrefab.transform.localPosition = Vector3.zero;
            CurrentWeaponPrefab.transform.localRotation = Quaternion.identity;
            CurrentWeapon = CurrentWeaponPrefab.GetComponent<WeaponController>();
            CurrentWeapon.SetOwner(Owner);
        }
    }
}
