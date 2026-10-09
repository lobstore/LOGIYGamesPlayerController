using GDS.Core;
using UnityEngine;

namespace GDS.Examples {

    public class EquipOnChar_CharController : MonoBehaviour {

        [SerializeField] Transform HeadSlot;
        [SerializeField] Transform WeaponSlot;

        public void Init(SetBag equipment) {
            equipment.OnItemChanged += OnEquipmentChanged;
        }

        private void OnEquipmentChanged(SetSlot slot) {
            Debug.Log($"item changed: {slot.Key}, {slot.Item}");
            var target = GetTtargetTransform(slot);
            if (target == null) return;
            target.Clear();
            if (slot.Item?.Base is not PrefabItemBase b) return;
            Debug.Log($"should add prefab to slot transform {b.Prefab}, {target}");

            var instance = Instantiate(b.Prefab, target);
            instance.transform.SetParent(target, true);
            instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        Transform GetTtargetTransform(SetSlot slot) => slot.Key switch {
            "Helmet" => HeadSlot,
            "Weapon" => WeaponSlot,
            _ => null
        };
    }
}