using LOGIYGames.Shared.Enums;
using AnimationEvent = LOGIYGames.Shared.Character.Events.AnimationEvent;
namespace LOGIYGames.CharacterCore
{
    public class ComboController
    {
        private AttackNodeSO currentAttack;
        private AttackNodeSO queuedAttack;

        private Actor character;
        public InputCommandBuffer CommandBuffer { get; private set; }

        public ComboMovesetSO comboMovesetSO { get; private set; }

        public ComboController(Actor character)
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
            StartAttack(comboMovesetSO.EntryAttack);
        }

        private void StartAttack(AttackNodeSO attack)
        {
            CommandBuffer.Clear();
            if (attack == null || string.IsNullOrWhiteSpace(attack.Animation.AnimationName))
            {
                ResetCombo();
                return;
            }
            currentAttack = attack;
        }

        // Find next attack node and queuing it
        private void ResolveTransition()
        {
            AttackTransition bestTransition = null;
            int bestMatchLength = 0;

            foreach (AttackTransition transition in currentAttack.Transitions)
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
                return;
            }

            queuedAttack = bestTransition.NextAttack;
        }
        // Check if we has next attack node queued and continue combo
        private void TryContinueCombo()
        {
            if (queuedAttack == null)
            {
                ResetCombo();
                return;
            }

            AttackNodeSO nextAttack = queuedAttack;
            queuedAttack = null;

            StartAttack(nextAttack);

        }

        public void Tick()
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
        private void ResetCombo()
        {
            queuedAttack = null;
            currentAttack = null;
            CommandBuffer.Clear();
        }
        public bool CanEnter()
        {
            return CommandBuffer.HasInput() && comboMovesetSO != null;
        }
        public bool CanExit()
        {
            return queuedAttack == null && currentAttack == null && !CommandBuffer.HasInput();
        }
        public void Exit()
        {
            ResetCombo();
        }
    }
}
