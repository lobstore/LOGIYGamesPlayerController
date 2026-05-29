using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public partial class ComboController
    {
        public AttackNodeSO CurrentAttack { get; private set; }
        public bool CanCancel { get; private set; }
        private CharacterModule character;
        private Animator animator;
        private InputCommandBuffer commandBuffer;
        private AttackNodeSO queuedAttack;

        public bool IsNextQueued { get; private set; }
        public ComboPhase Phase { get; private set; }
        public ComboController(CharacterModule owner, InputCommandBuffer commandBuffer)
        {
            character = owner;
            animator = owner.GetComponent<Animator>();
            this.commandBuffer = commandBuffer;
            SubscribeEvents();
        }
        private void SubscribeEvents()
        {
            character.EventBus.Subscribe<CharacterAnimationEvent>(e =>
            {
                OnAnimationEvent(e.AnimationEventType);
            });
        }
        public void BeginCombo(AttackNodeSO attack)
        {
            Phase = ComboPhase.Started;
            queuedAttack = null;
            StartAttack(attack);
        }
        private void StartAttack(AttackNodeSO attack)
        {
            commandBuffer.Clear();
            if (attack == null) return;
            CurrentAttack = attack;
            animator.applyRootMotion = attack.UseRootMotion;
            animator.CrossFade(attack.AnimationName, attack.CrossFade);
            if (attack.ForwardImpulse > 0)
                character.VelocityData.Locomotion += character.transform.forward * attack.ForwardImpulse;

        }
        public void OnAnimationEvent(AnimationEventType type)
        {
            switch (type)
            {
                case AnimationEventType.AttackStarted:
                    OnAttackStarted();
                    break;
                case AnimationEventType.EnableHitbox:
                    OnHitboxEnabled();
                    break;
                case AnimationEventType.DisableHitbox:
                    OnHiboxDisabled();
                    break;
                case AnimationEventType.OpenComboWindow:
                    OnComboWindowOpened();
                    break;
                case AnimationEventType.CloseComboWindow:
                    OnComboWindowClosed();
                    break;
                case AnimationEventType.OpenCancelWindow:
                    OnCancelWindowOpened();
                    break;
                case AnimationEventType.CloseCancelWindow:
                    OnCancelWindowClosed();
                    break;
                case AnimationEventType.AttackFinished:
                    OnAttackFinished();
                    break;
            }
        }
        #region Event Handlers
        private void OnHitboxEnabled()
        {

        }
        private void OnHiboxDisabled()
        {

        }
        private void OnComboWindowOpened()
        {

        }
        private void OnComboWindowClosed()
        {
            ResolveTransition();
        }
        private void OnCancelWindowOpened()
        {
            CanCancel = true;
        }
        private void OnCancelWindowClosed()
        {
            CanCancel = false;
        }
        private void OnAttackStarted()
        {

        }
        private void OnAttackFinished()
        {
            TryContinueCombo();
        }
        #endregion
        private void ResolveTransition()
        {
            AttackTransition bestTransition = null;

            int bestMatchLength = 0;

            foreach (AttackTransition transition in CurrentAttack.Transitions)
            {
                if (transition.Sequence == null
                    || transition.Sequence.Inputs == null
                    || transition.Sequence.Inputs.Count == 0)
                {
                    continue;
                }

                int matchLength = commandBuffer.GetMatchLength(transition.Sequence.Inputs);

                if (matchLength <= 0) continue;

                // choose best match
                if (matchLength > bestMatchLength)
                {
                    bestMatchLength = matchLength;
                    bestTransition = transition;
                }
            }

            if (bestTransition == null)
            {
                IsNextQueued = false;
                return;
            }
            queuedAttack = bestTransition.NextAttack;
            IsNextQueued = true;
        }
        private void TryContinueCombo()
        {
            if (queuedAttack == null)
            {
                Phase = ComboPhase.Finished;
                commandBuffer.Clear();
                return;
            }
            StartAttack(queuedAttack);
            queuedAttack = null;
        }
        public bool IsFinished()
        {
            return Phase == ComboPhase.Finished;
        }
        public void Reset()
        {
            queuedAttack = null;
            CurrentAttack = null;
            CanCancel = false;
            Phase = ComboPhase.None;
            IsNextQueued = false;
        }
    }
}
