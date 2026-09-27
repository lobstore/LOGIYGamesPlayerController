using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using System;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class ComboController : MonoBehaviour
    {
        private AttackNodeSO currentAttack;
        private AttackNodeSO queuedAttack;

        private Actor character;

        private float attackTime;

        // Чтобы ResolveTransition() был вызван
        // только один раз после окончания ComboInputWindow.
        private bool comboInputWindowProcessed;

        public InputCommandBuffer CommandBuffer { get; private set; }

        public ComboMovesetSO ComboMovesetSO { get; private set; }
        [SerializeField] private ComboMovesetSO comboMovesetSO;

        private void Awake()
        {
            ComboMovesetSO = comboMovesetSO;
            character = GetComponent<Actor>();

            CommandBuffer = new InputCommandBuffer();

            SubscribeEvents();
        }


        private void SubscribeEvents()
        {
            character.EventBus.Subscribe<WeaponEquipEvent>(OnWeaponEquip);
        }

        private void OnWeaponEquip(WeaponEquipEvent evt)
        {
            switch (evt.WeaponEquipState)
            {
                case WeaponEquipState.Equiped:

                    ComboMovesetSO = evt.WeaponData.ComboSet;

                    break;

                case WeaponEquipState.Unequiped:

                    ComboMovesetSO = null;

                    ResetCombo();

                    break;
            }
        }

        public void BeginCombo()
        {
            if (ComboMovesetSO == null)
                return;

            if (ComboMovesetSO.EntryAttack == null)
                return;

            StartAttack(ComboMovesetSO.EntryAttack);
        }

        private void StartAttack(AttackNodeSO attack)
        {
            if (!IsAttackValid(attack))
            {
                ResetCombo();
                return;
            }

            currentAttack = attack;

            queuedAttack = null;

            attackTime = 0f;

            comboInputWindowProcessed = false;

            CommandBuffer.Clear();

            // Здесь запускаем animation/state:
            //
            character.EventBus.Publish(new ComboAttackEvent()
            {
                AnimationData = attack.Animation
            });
            //     attack.Animation.AnimationName);
        }

        private bool IsAttackValid(AttackNodeSO attack)
        {
            if (attack == null)
                return false;

            if (string.IsNullOrWhiteSpace(
                    attack.Animation.AnimationName))
            {
                return false;
            }

            if (attack.TotalDuration <= 0f)
                return false;

            if (!attack.ComboInputWindow.IsValid())
                return false;

            if (!attack.DodgeCancelWindow.IsValid())
                return false;

            if (attack.ComboInputWindow.End >
                attack.TotalDuration)
            {
                return false;
            }

            if (attack.DodgeCancelWindow.End >
                attack.TotalDuration)
            {
                return false;
            }

            return true;
        }

        public void Tick()
        {
            if (currentAttack == null)
                return;

            attackTime += Time.deltaTime;

            ProcessInputs();

            ProcessComboWindow();

            ProcessAttackEnd();
        }

        private void ProcessInputs()
        {
            ProcessAttackInput();
        }

        private void ProcessAttackInput()
        {
            if (!character.Input.AttackPressed)
                return;

            if (!currentAttack.ComboInputWindow
                    .Contains(attackTime))
            {
                return;
            }

            CommandBuffer.AddCommand(
                new AttackInputCommand(
                    AttackInputType.Light));
        }

        private void ProcessComboWindow()
        {
            if (comboInputWindowProcessed)
                return;

            if (attackTime < currentAttack.ComboInputWindow.End)
                return;

            comboInputWindowProcessed = true;

            ResolveTransition();

            if (queuedAttack != null)
            {
                StartQueuedAttack();
            }
        }

        private void ResolveTransition()
        {
            AttackTransition bestTransition = null;

            int bestMatchLength = 0;

            foreach (AttackTransition transition
                     in currentAttack.Transitions)
            {
                if (transition == null)
                    continue;

                if (transition.Sequence == null)
                    continue;

                if (transition.Sequence.Inputs == null)
                    continue;

                if (transition.Sequence.Inputs.Count == 0)
                    continue;

                if (transition.NextAttack == null)
                    continue;

                int matchLength =
                    CommandBuffer.GetMatchLength(
                        transition.Sequence.Inputs);

                if (matchLength <= 0)
                    continue;

                if (matchLength > bestMatchLength)
                {
                    bestMatchLength = matchLength;

                    bestTransition = transition;
                }
            }

            if (bestTransition == null)
            {
                queuedAttack = null;
                return;
            }

            queuedAttack = bestTransition.NextAttack;
        }

        private void StartQueuedAttack()
        {
            if (queuedAttack == null)
                return;

            AttackNodeSO nextAttack = queuedAttack;

            queuedAttack = null;

            StartAttack(nextAttack);
        }

        private void ProcessAttackEnd()
        {
            if (currentAttack == null)
                return;

            if (attackTime < currentAttack.TotalDuration)
                return;

            ResetCombo();
        }

        private void ResetCombo()
        {
            currentAttack = null;

            queuedAttack = null;

            attackTime = 0f;

            comboInputWindowProcessed = false;

            CommandBuffer.Clear();
        }

        public bool CanEnter()
        {
            if (ComboMovesetSO == null)
                return false;

            if (ComboMovesetSO.EntryAttack == null)
                return false;

            return character.Input.AttackPressed;
        }

        public bool CanExit()
        {
            return currentAttack == null &&
                   queuedAttack == null &&
                   !CommandBuffer.HasInput();
        }

        public void Exit()
        {
            ResetCombo();
        }

        public AttackNodeSO GetCurrentAttack()
        {
            return currentAttack;
        }

        public AttackNodeSO GetQueuedAttack()
        {
            return queuedAttack;
        }

        public float GetAttackTime()
        {
            return attackTime;
        }

        public float GetAttackNormalizedTime()
        {
            if (currentAttack == null)
                return 0f;

            if (currentAttack.TotalDuration <= 0f)
                return 0f;

            return Mathf.Clamp01(
                attackTime / currentAttack.TotalDuration);
        }

        public bool IsComboInputWindowActive()
        {
            if (currentAttack == null)
                return false;

            return currentAttack.ComboInputWindow
                .Contains(attackTime);
        }

        public bool IsDodgeCancelWindowActive()
        {
            if (currentAttack == null)
                return false;

            return currentAttack.DodgeCancelWindow
                .Contains(attackTime);
        }
    }

    [Serializable]
    public struct TimeWindow
    {
        [Min(0f)]
        public float Start;

        [Min(0f)]
        public float End;

        public bool Contains(float time)
        {
            return time >= Start && time <= End;
        }

        public bool IsValid()
        {
            return Start >= 0f && End >= Start;
        }
    }
}

