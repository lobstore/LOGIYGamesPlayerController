using LOGIYGames.Shared.Enums;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{

    [CreateAssetMenu(menuName = "Item/WeaponData")]
    public class WeaponData : ItemData
    {
        public DamageData BaseDamage;

        public GameObject Prefab;

        public RuntimeAnimatorController AnimatorOverride;

        public ComboData ComboData;

        public WeaponSlotType SlotType;

        public bool TwoHandsRequired;
    }
}
