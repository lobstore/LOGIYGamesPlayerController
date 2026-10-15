
using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace LOGIYGames
{
    public class EquipmentController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WeaponSlot rightHandSlot;

        [Header("Weapons")]
        [SerializeField] private List<WeaponData> weapons = new();

        private Actor character;
        private MantlingController mantlingController;

        private List<WeaponData> rightHandWeapons = new();

        private int selectedIndex;
        private bool isWeaponEquipped;

        private void Awake()
        {
            character = GetComponent<Actor>();
            mantlingController = GetComponent<MantlingController>();

            BuildRightHandWeaponList();
        }

        private void Start()
        {
            if (mantlingController != null)
            {
                mantlingController.OnMantlingStart.AddListener(OnMantlingStart);
                mantlingController.OnMantlingEnd.AddListener(OnMantlingEnd);
            }

            // В начале ничего не экипировано.
            rightHandSlot.UnloadModel();
            isWeaponEquipped = false;

            // Выбираем первое доступное оружие.
            selectedIndex = 0;
        }

        private void Update()
        {
            HandleWeaponSelection();
            HandleEquipInput();
        }

        private void OnDestroy()
        {
            if (mantlingController != null)
            {
                mantlingController.OnMantlingStart.RemoveListener(OnMantlingStart);
                mantlingController.OnMantlingEnd.RemoveListener(OnMantlingEnd);
            }
        }

        private void BuildRightHandWeaponList()
        {
            rightHandWeapons = weapons
                .Where(weapon =>
                    weapon != null &&
                    weapon.SlotType == WeaponSlotType.RightHand)
                .ToList();

            selectedIndex = Mathf.Clamp(
                selectedIndex,
                0,
                Mathf.Max(0, rightHandWeapons.Count - 1));
        }

        private void HandleWeaponSelection()
        {
            if (rightHandWeapons.Count == 0)
                return;

            float scroll = Input.mouseScrollDelta.y;

            if (Mathf.Approximately(scroll, 0f))
                return;

            if (scroll > 0f)
            {
                SelectNextWeapon();
            }
            else
            {
                SelectPreviousWeapon();
            }
        }

        private void HandleEquipInput()
        {
            if (!Input.GetKeyDown(KeyCode.Tab))
                return;

            if (rightHandWeapons.Count == 0)
                return;

            if (isWeaponEquipped)
            {
                UnloadRightHandWeapon();
            }
            else
            {
                LoadRightHandWeapon();
            }
        }

        private void SelectNextWeapon()
        {
            selectedIndex++;

            if (selectedIndex >= rightHandWeapons.Count)
                selectedIndex = 0;

            ChangeWeaponIfEquipped();
        }

        private void SelectPreviousWeapon()
        {
            selectedIndex--;

            if (selectedIndex < 0)
                selectedIndex = rightHandWeapons.Count - 1;

            ChangeWeaponIfEquipped();
        }

        private void ChangeWeaponIfEquipped()
        {
            if (!isWeaponEquipped)
                return;

            LoadRightHandWeapon();
        }

        public void LoadRightHandWeapon()
        {
            if (rightHandWeapons.Count == 0)
                return;

            WeaponData weaponToEquip = rightHandWeapons[selectedIndex];

            if (weaponToEquip == null)
                return;

            if (isWeaponEquipped && rightHandSlot.CurrentWeapon == weaponToEquip)
            {
                return;
            }
            if (isWeaponEquipped)
            {
                UnloadRightHandWeapon();
            }

            rightHandSlot.LoadModel(weaponToEquip);
            isWeaponEquipped = true;

            PublishWeaponEvent(
                WeaponEquipState.Equiped,
                rightHandSlot.CurrentWeapon);
        }

        public void UnloadRightHandWeapon()
        {
            if (!isWeaponEquipped)
                return;


            PublishWeaponEvent( WeaponEquipState.Unequiped, rightHandSlot.CurrentWeapon);

            rightHandSlot.UnloadModel();
            isWeaponEquipped = false;
        }

        private void PublishWeaponEvent( WeaponEquipState state,WeaponController weapon)
        {
            if (character == null || character.EventBus == null)
                return;

            character.EventBus.Publish(new WeaponEquipEvent
            {
                WeaponSlotType = WeaponSlotType.RightHand,
                WeaponEquipState = state,
                Weapon = weapon
            });
        }

        private void OnMantlingStart()
        {
            if (!isWeaponEquipped)
                return;
            PublishWeaponEvent(WeaponEquipState.Unequiped, rightHandSlot.CurrentWeapon);
            rightHandSlot.UnloadModel();

        }

        private void OnMantlingEnd()
        {
            if (!isWeaponEquipped)
                return;
            if (rightHandSlot.CurrentWeaponPrefab == null)
            {
                rightHandSlot.LoadModel(rightHandWeapons[selectedIndex]);
                PublishWeaponEvent(WeaponEquipState.Equiped, rightHandSlot.CurrentWeapon);
            }

        }
    }

    [Serializable]
    public class WeaponEquipEvent : EventBase
    {
        public WeaponSlotType WeaponSlotType { get; set; }
        public WeaponEquipState WeaponEquipState { get; set; }
        public WeaponController Weapon;
    }
}
