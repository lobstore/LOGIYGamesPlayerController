using LOGIYGames.CharacterCore;

namespace LOGIYGames.Movement
{
    public class CrouchMovementState : BaseCharacterMovementState
    {
        protected float StandingHeight;
        protected float CrouchHeight;

        public CrouchMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            StandingHeight = _character.Height;
            CrouchHeight = StandingHeight * 0.5f;
        }

        public override void Enter()
        {
            base.Enter();
            _character.Height = CrouchHeight;
        }

        public override void Exit()
        {
            base.Exit();
            _character.Height = StandingHeight;
        }
    }

}
