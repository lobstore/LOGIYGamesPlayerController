using LOGIYGames.Shared.Enums;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{

    [CreateAssetMenu]
    public class WeaponDataSO : ScriptableObject
    {
        public GameObject Prefab;

        public RuntimeAnimatorController AnimatorOverride;

        public ComboMovesetSO ComboSet;

        public WeaponSlotType SlotType;

        public bool TwoHandsRequired;
    }
}
