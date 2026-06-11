using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Enums;

namespace LOGIYGames
{
    public class ComboMovementState : CharacterMovementState
    {
        ComboController combo;
        WeaponController weapon;
        public ComboMovementState(CharacterModule character, MovementStateData data) : base(character, data)
        {
            combo = character.ComboController;
            weapon = character.WeaponController;
        }

        // ========================================================
        // ENTER
        // ========================================================

        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new NoneMovement();
            _character.RotationStrategy = new NoneRotation(_character);
            ComboMovesetSO moveset = weapon.GetWeaponCombo();

            if (moveset == null)
                return;

            _character.ComboController.BeginCombo(moveset.EntryAttack);
            _character.ResetInput();
        }
        public override void Exit()
        {
            base.Exit();

            combo.ResetCombo();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            ReadInput();
        }

        private void ReadInput()
        {
            if (_character.Input.AttackPressed)
            {
                combo.AddCommand(new AttackInputCommand(AttackInputType.Light));
            }

            if (_character.Input.EvadePressed)
            {
                combo.AddCommand(new AttackInputCommand(AttackInputType.Heavy));
            }
        }
        public bool CanExit()
        {
            return combo.IsFinished();
        }


    }
}