
using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LOGIYGames
{
    public class EquipmentController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WeaponSlot rightHandSlot;

        [Header("Weapons")]
        [SerializeField] private List<WeaponDataSO> weapons = new();

        private Actor character;
        private MantlingController mantlingController;

        private List<WeaponDataSO> rightHandWeapons = new();

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

            WeaponDataSO weaponToEquip = rightHandWeapons[selectedIndex];

            if (weaponToEquip == null)
                return;

            // Если уже экипировано именно это оружие — ничего не делаем.
            if (isWeaponEquipped &&
                rightHandSlot.CurrentWeapon == weaponToEquip)
            {
                return;
            }

            // Сначала снимаем текущее оружие.
            if (isWeaponEquipped)
            {
                UnloadRightHandWeapon();
            }

            rightHandSlot.LoadModel(weaponToEquip);
            isWeaponEquipped = true;

            PublishWeaponEvent(
                WeaponEquipState.Equiped,
                weaponToEquip);
        }

        public void UnloadRightHandWeapon()
        {
            if (!isWeaponEquipped)
                return;

            // Сохраняем ссылку ДО UnloadModel(),
            // потому что после него CurrentWeapon может стать null.
            WeaponDataSO currentWeapon = rightHandSlot.CurrentWeapon;

            PublishWeaponEvent(
                WeaponEquipState.Unequiped,
                currentWeapon);

            rightHandSlot.UnloadModel();
            isWeaponEquipped = false;
        }

        private void PublishWeaponEvent(
            WeaponEquipState state,
            WeaponDataSO weapon)
        {
            if (character == null || character.EventBus == null)
                return;

            character.EventBus.Publish(new WeaponEquipEvent
            {
                WeaponSlotType = WeaponSlotType.RightHand,
                WeaponEquipState = state,
                WeaponData = weapon
            });
        }

        private void OnMantlingStart()
        {
            if (!isWeaponEquipped)
                return;
            WeaponDataSO currentWeapon = rightHandSlot.CurrentWeapon;
            // При mantle модель временно убираем,
            // но состояние equipped сохраняем.
            PublishWeaponEvent(WeaponEquipState.Unequiped,currentWeapon);
            rightHandSlot.UnloadModel();

        }

        private void OnMantlingEnd()
        {
            if (!isWeaponEquipped)
                return;

            // После mantle возвращаем то же оружие.
            if (rightHandSlot.CurrentWeaponPrefab == null)
            {
                WeaponDataSO currentWeapon = rightHandWeapons[selectedIndex];

                rightHandSlot.LoadModel(currentWeapon);
                // При mantle модель временно убираем,
                // но состояние equipped сохраняем.
                PublishWeaponEvent(WeaponEquipState.Equiped, currentWeapon);
            }

        }
    }

    [Serializable]
    public class WeaponEquipEvent : EventBase
    {
        public WeaponSlotType WeaponSlotType { get; set; }
        public WeaponEquipState WeaponEquipState { get; set; }
        public WeaponDataSO WeaponData;
    }
}
