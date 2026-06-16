using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class ComboController : MonoBehaviour
    {
        public AttackNodeSO CurrentAttack { get; private set; }
        private AttackNodeSO queuedAttack;
        public bool CanCancel { get; private set; }
        public bool IsNextQueued { get; private set; }
        public ComboPhase Phase { get; private set; }

        private CharacterModule character;
        private Animator animator;
        private InputCommandBuffer commandBuffer;

        public ComboMovesetSO comboMovesetSO {  get; private set; }
        private void Awake()
        {
            character = GetComponent<CharacterModule>();
            animator = GetComponent<Animator>();
            commandBuffer = new InputCommandBuffer();
            GetComponent<ComboBufferDebugView>().Buffer = commandBuffer;

        }

        private void Start()
        {
            SubscribeEvents();
        }
        public void AddCommand(IInputCommand input)
        {
            commandBuffer.AddCommand(input);
        }
        private void SubscribeEvents()
        {
            character.EventBus.Subscribe<ComboAnimationEvent>(e =>
            {
                OnAnimationEvent(e.ComboEventType);
            });
            character.EventBus.Subscribe<WeaponEquipEvent>((evt) =>
            {
                Debug.Log(evt.WeaponEquipState);
                Debug.Log(evt.WeaponData);
                Debug.Log(evt.WeaponData);
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
            commandBuffer.Clear();

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

            int stateHash = Animator.StringToHash(attack.Animation.AnimationName);

            // Проверяем наличие состояния в Base Layer
            if (!animator.HasState(0, stateHash))
            {
                Debug.LogError(
                    $"ComboController: Animator state '{attack.Animation.AnimationName}' not found.");

                FinishCombo();
                return;
            }

            CurrentAttack = attack;

            animator.applyRootMotion = attack.Animation.UseRootMotion;

            animator.CrossFade(
                attack.Animation.AnimationName,
                attack.Animation.CrossFade);

            if (attack.ForwardImpulse > 0f)
            {
                character.VelocityData.Locomotion +=
                    character.transform.forward * attack.ForwardImpulse;
            }
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
                    commandBuffer.GetMatchLength(
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
                commandBuffer.AddCommand(new AttackInputCommand(AttackInputType.Light));
            }

            if (character.Input.EvadePressed)
            {
                commandBuffer.AddCommand(new AttackInputCommand(AttackInputType.Heavy));
            }
        }
        public void ResetCombo()
        {
            queuedAttack = null;
            CurrentAttack = null;

            CanCancel = false;
            IsNextQueued = false;
            commandBuffer.Clear();
            Phase = ComboPhase.None;
        }
    }
}
