using LOGIYGames.Shared.Enums;
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

        private readonly CharacterModule
            character;

        private readonly Animator
            animator;

        private AttackNodeSO
            queuedAttack;

        private bool comboWindowOpened;
        public bool CanCancel
        {
            get;
            private set;
        }
        private bool finished;

        public ComboController(
            CharacterModule owner)
        {
            character = owner;

            animator =
                owner.GetComponent<Animator>();
        }

        // ========================================================
        // START COMBO
        // ========================================================

        public void StartCombo(
            AttackNodeSO attack)
        {
            finished = false;

            queuedAttack = null;

            PlayAttack(attack);
        }

        // ========================================================
        // PLAY ATTACK
        // ========================================================

        private void PlayAttack(
            AttackNodeSO attack)
        {
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

        // ========================================================
        // INPUT
        // ========================================================

        public void HandleInput(
            AttackInputType input)
        {
            if (!comboWindowOpened)
                return;

            foreach (AttackTransition
                     transition
                     in CurrentAttack
                         .Transitions)
            {
                if (transition.Input
                    != input)
                    continue;

                queuedAttack =
                    transition.NextAttack;

                return;
            }
        }

        // ========================================================
        // EVENTS
        // ========================================================

        public void OnAnimationEvent(
            AnimationEventType type)
        {
            switch (type)
            {
                case AnimationEventType
                    .EnableHitbox:

                    //Weapon.Hitbox

                    break;

                case AnimationEventType
                    .DisableHitbox:

                    //Weapon.Hitbox

                    break;

                case AnimationEventType
                    .OpenComboWindow:

                    comboWindowOpened = true;

                    break;

                case AnimationEventType
                    .CloseComboWindow:

                    comboWindowOpened = false;

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

        // ========================================================
        // NEXT ATTACK
        // ========================================================

        private void TryContinueCombo()
        {
            if (queuedAttack == null)
            {
                finished = true;

                return;
            }

            PlayAttack(queuedAttack);

            queuedAttack = null;
        }

        // ========================================================
        // HELPERS
        // ========================================================

        public bool IsFinished()
        {
            return finished;
        }

        // ========================================================
        // STOP
        // ========================================================

        public void Stop()
        {
            comboWindowOpened = false;

            queuedAttack = null;

            CurrentAttack = null;

            finished = false;

        }
    }
}
