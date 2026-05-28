using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Enums;

namespace LOGIYGames
{
    public class ComboMovementState
        : CharacterMovementState
    {

        public ComboMovementState(
            CharacterModule character,
            MovementStateData data)
            : base(character, data)
        {
            ;
        }

        // ========================================================
        // ENTER
        // ========================================================

        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new NoneMovement();
            _character.RotationStrategy = new NoneRotation(_character);
            ComboMovesetSO moveset =
                _character
                    .WeaponController
                    .GetWeaponCombo();

            if (moveset == null)
                return;

            _character.ComboController.StartCombo(
    moveset.EntryAttack,
    new[]
    {
        AttackInputType.Light
    });
            _character.ResetInput();
        }

        // ========================================================
        // EXIT
        // ========================================================

        public override void Exit()
        {
            base.Exit();

            _character.ComboController.Stop();
        }

        // ========================================================
        // UPDATE
        // ========================================================

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            ReadInput();
        }

        // ========================================================
        // INPUT
        // ========================================================

        private void ReadInput()
        {
            if (_character
                    .Input
                    .AttackPressed)
            {
                _character.ComboBuffer.AddCommand(new AttackInputCommand(AttackInputType.Light));
            }

            if (_character
                    .Input
                    .EvadePressed)
            {
                _character.ComboBuffer.AddCommand(new AttackInputCommand(AttackInputType.Heavy));
            }
        }
        public bool CanCancel()
        {
            return _character.ComboController.CanCancel;
        }


    }
}