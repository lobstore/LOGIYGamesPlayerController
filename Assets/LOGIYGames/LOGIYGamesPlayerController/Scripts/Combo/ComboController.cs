using LOGIYGames.Shared.Enums;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class ComboController
    {
        public AttackNodeSO CurrentAttack
        {
            get;
            private set;
        }

        public bool CanCancel
        {
            get;
            private set;
        }

        // =====================================================
        // REFERENCES
        // =====================================================

        private readonly CharacterModule
            character;

        private readonly Animator
            animator;

        private readonly InputCommandBuffer
            commandBuffer;

        // =====================================================
        // STATE
        // =====================================================

        private AttackNodeSO
            queuedAttack;

        private IReadOnlyList<AttackInputType>
            queuedSequence;

        private bool comboWindowOpened;

        private bool finished;

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public ComboController(
            CharacterModule owner,
            InputCommandBuffer commandBuffer)
        {
            character = owner;

            animator =
                owner.GetComponent<Animator>();

            this.commandBuffer =
                commandBuffer;
        }

        // =====================================================
        // START COMBO
        // =====================================================

        public void StartCombo(
            AttackNodeSO attack,
            IReadOnlyList<AttackInputType>
                usedSequence = null)
        {
            finished = false;

            queuedAttack = null;

            queuedSequence = null;
            PlayAttack(attack);
        }

        // =====================================================
        // PLAY ATTACK
        // =====================================================

        private void PlayAttack(
            AttackNodeSO attack)
        {
            commandBuffer.Clear();
            if (attack == null)
                return;

            CurrentAttack = attack;

            animator.applyRootMotion =
                attack.UseRootMotion;

            animator.CrossFade(
                attack.AnimationName,
                attack.CrossFade);

            if (attack.ForwardImpulse > 0)
            {
                character.VelocityData
                    .Locomotion +=
                    character.transform.forward
                    * attack.ForwardImpulse;
            }

        }

        // =====================================================
        // INPUT
        // =====================================================

        public void HandleInput(
            AttackInputType input)
        {
            commandBuffer.AddCommand(
                new AttackInputCommand(input));
        }

        // =====================================================
        // EVENTS
        // =====================================================

        public void OnAnimationEvent(
            AnimationEventType type)
        {
            switch (type)
            {
                case AnimationEventType
                    .EnableHitbox:

                    break;

                case AnimationEventType
                    .DisableHitbox:

                    break;

                case AnimationEventType
                    .OpenComboWindow:

                    comboWindowOpened = true;

                    break;

                case AnimationEventType
                    .CloseComboWindow:

                    comboWindowOpened = false;

                    ResolveTransition();


                    break;

                case AnimationEventType
                    .EnableCancelWindow:

                    CanCancel = true;

                    break;

                case AnimationEventType
                    .DisableCancelWindow:

                    CanCancel = false;

                    break;

                case AnimationEventType
                    .EndAnimation:

                    TryContinueCombo();

                    break;
            }
        }
        private void ResolveTransition()
        {
            AttackTransition
                bestTransition = null;

            int bestMatchLength = 0;

            foreach (AttackTransition
                     transition
                     in CurrentAttack
                         .Transitions)
            {
                if (transition.Sequence == null
                    || transition.Sequence.Inputs == null
                    || transition.Sequence.Inputs.Count == 0)
                {
                    continue;
                }

                int matchLength =
                    commandBuffer.GetMatchLength(
                        transition.Sequence.Inputs);

                if (matchLength <= 0)
                    continue;

                // choose best match
                if (matchLength
                    > bestMatchLength)
                {
                    bestMatchLength =
                        matchLength;

                    bestTransition =
                        transition;
                }
            }

            if (bestTransition == null)
                return;

            queuedAttack =
                bestTransition.NextAttack;

            queuedSequence =
                bestTransition.Sequence
                    .Inputs;
        }
        // =====================================================
        // NEXT ATTACK
        // =====================================================

        private void TryContinueCombo()
        {
            if (queuedAttack == null)
            {
                finished = true;

                commandBuffer.Clear();

                return;
            }

            PlayAttack(queuedAttack);



            queuedAttack = null;
        }

        // =====================================================
        // HELPERS
        // =====================================================

        public bool IsFinished()
        {
            return finished;
        }

        // =====================================================
        // STOP
        // =====================================================

        public void Stop()
        {
            comboWindowOpened = false;

            queuedAttack = null;

            queuedSequence = null;

            CurrentAttack = null;

            finished = false;

            CanCancel = false;
        }
    }
}
