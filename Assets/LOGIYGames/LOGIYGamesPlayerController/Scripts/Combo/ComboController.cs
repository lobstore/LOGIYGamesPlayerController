using LOGIYGames.Shared.Enums;
using AnimationEvent = LOGIYGames.Shared.Character.Events.AnimationEvent;
namespace LOGIYGames.CharacterCore
{
    public class ComboController
    {
        public AttackNodeSO CurrentAttack { get; private set; }
        private AttackNodeSO queuedAttack;
        public bool CanCancel { get; private set; }
        public bool IsNextQueued { get; private set; }
        public ComboPhase Phase { get; private set; }

        private Character character;
        public InputCommandBuffer CommandBuffer { get; private set; }

        public ComboMovesetSO comboMovesetSO { get; private set; }

        public ComboController(Character character)
        {
            this.character = character;
            CommandBuffer = new InputCommandBuffer();
            SubscribeEvents();
        }
        public void AddCommand(IComboInputCommand input)
        {
            CommandBuffer.AddCommand(input);
        }
        private void SubscribeEvents()
        {
            character.EventBus.Subscribe<WeaponEquipEvent>((evt) =>
            {
                switch (evt.WeaponEquipState)
                {
                    case WeaponEquipState.Equiped:
                        comboMovesetSO = evt.WeaponData.ComboSet;
                        break;
                    case WeaponEquipState.Unequiped:
                        comboMovesetSO = null;
                        ResetCombo();
                        break;
                    default:
                        break;
                }

            });
        }

        public void BeginCombo()
        {
            ResetCombo();

            Phase = ComboPhase.Started;

            StartAttack(comboMovesetSO.EntryAttack);
        }

        private void StartAttack(AttackNodeSO attack)
        {
            CommandBuffer.Clear();

            if (attack == null)
            {
                FinishCombo();
                return;
            }


            if (string.IsNullOrWhiteSpace(attack.Animation.AnimationName))
            {
                FinishCombo();
                return;
            }


            CurrentAttack = attack;


            character.EventBus.Publish(new ComboAttackEvent
            {
                AnimationData = attack.Animation
            });

        }

        public void OnAnimationEvent(ComboEventType type)
        {
            switch (type)
            {
                case ComboEventType.AttackStarted:
                    OnAttackStarted();
                    break;

                case ComboEventType.EnableHitbox:
                    OnHitboxEnabled();
                    break;

                case ComboEventType.DisableHitbox:
                    OnHitboxDisabled();
                    break;

                case ComboEventType.OpenComboWindow:
                    OnComboWindowOpened();
                    break;

                case ComboEventType.CloseComboWindow:
                    OnComboWindowClosed();
                    break;

                case ComboEventType.OpenCancelWindow:
                    OnCancelWindowOpened();
                    break;

                case ComboEventType.CloseCancelWindow:
                    OnCancelWindowClosed();
                    break;

                case ComboEventType.AttackFinished:
                    OnAttackFinished();
                    break;
            }
        }

        #region Event Handlers

        private void OnAttackStarted()
        {
        }

        private void OnHitboxEnabled()
        {
        }

        private void OnHitboxDisabled()
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
                if (transition.Sequence == null ||
                    transition.Sequence.Inputs == null ||
                    transition.Sequence.Inputs.Count == 0)
                {
                    continue;
                }

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
                FinishCombo();
                return;
            }

            AttackNodeSO nextAttack = queuedAttack;

            queuedAttack = null;
            IsNextQueued = false;

            StartAttack(nextAttack);

        }

        private void FinishCombo()
        {

            ResetCombo();

            Phase = ComboPhase.Finished;

        }

        public bool IsFinished()
        {
            return Phase == ComboPhase.Finished;
        }
        public void ReadInput()
        {
            if (character.Input.AttackPressed)
            {
                CommandBuffer.AddCommand(new AttackInputCommand(AttackInputType.Light));
            }

            if (character.Input.EvadePressed)
            {
                CommandBuffer.AddCommand(new AttackInputCommand(AttackInputType.Heavy));
            }
        }
        public void ResetCombo()
        {
            queuedAttack = null;
            CurrentAttack = null;

            CanCancel = false;
            IsNextQueued = false;
            CommandBuffer.Clear();
            Phase = ComboPhase.None;
        }
    }
    public class ComboAttackEvent : AnimationEvent
    {

    }
}
