using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames
{
    public class CharacterEquipmentController : MonoBehaviour
    {
        [SerializeField] WeaponSlot rightHandSlot;
        [SerializeField] WeaponSlot leftHandSlot;

        [SerializeField] WeaponDataSO defaultWeapon;

        CharacterModule characterModule;
        bool isWeaponRightHandWasLoaded;
        bool isWeaponLeftHandWasLoaded;
        private void Awake()
        {
            characterModule = GetComponent<CharacterModule>();
        }
        private void Start()
        {
            UnloadRightHandWeapon();
            UnloadLeftHandWeapon();
            Debug.Log(characterModule.GetMovementState<MantlingMovementState>());
            characterModule.GetMovementState<MantlingMovementState>().OnMantlingStart.AddListener(() =>
            {
                rightHandSlot.UnloadModel();
                leftHandSlot.UnloadModel();
            });
            characterModule.GetMovementState<MantlingMovementState>().OnMantlingEnd.AddListener(() =>
            {
                if (isWeaponLeftHandWasLoaded)
                    LoadLeftHandWeapon();
                if (isWeaponRightHandWasLoaded)
                    LoadRightHandWeapon();
            });
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!isWeaponLeftHandWasLoaded && !defaultWeapon.TwoHandsRequired)
                {
                    LoadLeftHandWeapon();
                }
                else
                {
                    UnloadLeftHandWeapon();
                }
                if (!isWeaponRightHandWasLoaded)
                {
                    LoadRightHandWeapon();
                }
                else
                {
                    UnloadRightHandWeapon();
                }
            }
        }
        public void UnloadRightHandWeapon()
        {
            rightHandSlot.UnloadModel();
            characterModule.EventBus.Publish(new WeaponEquipEvent
            {
                WeaponSlotType = WeaponSlotType.RightHand,
                WeaponEquipState = WeaponEquipState.Unequiped,
                WeaponData = defaultWeapon
            });
            isWeaponRightHandWasLoaded = false;
        }
        public void UnloadLeftHandWeapon()
        {
            leftHandSlot.UnloadModel();
            characterModule.EventBus.Publish(new WeaponEquipEvent
            {
                WeaponSlotType = WeaponSlotType.LeftHand,
                WeaponEquipState = WeaponEquipState.Unequiped,
                WeaponData = defaultWeapon
            });
            isWeaponLeftHandWasLoaded = false;
        }
        public void LoadRightHandWeapon()
        {
            UnloadRightHandWeapon();
            var obj = Instantiate(defaultWeapon.Prefab);
            rightHandSlot.LoadModel(obj);
            characterModule.EventBus.Publish(new WeaponEquipEvent
            {
                WeaponSlotType = WeaponSlotType.RightHand,
                WeaponEquipState = WeaponEquipState.Equiped,
                WeaponData = defaultWeapon
            });
            isWeaponRightHandWasLoaded = true;
        }
        public void LoadLeftHandWeapon()
        {
            UnloadLeftHandWeapon();
            var obj = Instantiate(defaultWeapon.Prefab);
            leftHandSlot.LoadModel(obj);
            characterModule.EventBus.Publish(new WeaponEquipEvent
            {
                WeaponSlotType = WeaponSlotType.LeftHand,
                WeaponEquipState = WeaponEquipState.Equiped,
                WeaponData = defaultWeapon
            });
            isWeaponLeftHandWasLoaded = true;
        }
    }

    public class WeaponEquipEvent : EventBase
    {
        public WeaponSlotType WeaponSlotType { get; set; }
        public WeaponEquipState WeaponEquipState { get; set; }
        public WeaponDataSO WeaponData;
    }
}
