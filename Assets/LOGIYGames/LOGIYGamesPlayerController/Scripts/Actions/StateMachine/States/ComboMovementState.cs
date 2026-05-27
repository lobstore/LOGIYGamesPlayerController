using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Enums;

namespace LOGIYGames
{
    public class ComboMovementState
        : CharacterMovementState
    {
        private readonly ComboController
            _comboController;

        public ComboMovementState(
            Character character,
            MovementStateData data)
            : base(character, data)
        {
            _comboController =
                new ComboController(character);
        }

        // =====================================================
        // ENTER
        // =====================================================

        public override void Enter()
        {
            base.Enter();
            _character.MovementStrategy = new NoneMovement();
            _character.RotationStrategy = new NoneRotation(_character);
            ComboMovesetSO moveset =
                _character
                    .WeaponController
                    .GetMoveset();

            if (moveset == null)
                return;

            _comboController.StartCombo(
                moveset.EntryAttack);
        }

        // =====================================================
        // EXIT
        // =====================================================

        public override void Exit()
        {
            base.Exit();

            _comboController.Stop();
        }

        // =====================================================
        // UPDATE
        // =====================================================

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            ReadInput();

            _comboController.Tick();
        }

        // =====================================================
        // INPUT
        // =====================================================

        private void ReadInput()
        {
            if (_character.Input.AttackPressed)
            {
                _character.ComboBuffer
                    .BufferInput(
                        AttackInputType.Light);
            }

            if (_character.Input.HeavyAttackPressed)
            {
                _character.ComboBuffer
                    .BufferInput(
                        AttackInputType.Heavy);
            }
        }

        // =====================================================
        // HELPERS
        // =====================================================

        public bool IsFinished()
        {
            return _comboController
                .IsFinished();
        }

        public bool CanCancel()
        {
            return _comboController
                .CanCancel();
        }
    }
}