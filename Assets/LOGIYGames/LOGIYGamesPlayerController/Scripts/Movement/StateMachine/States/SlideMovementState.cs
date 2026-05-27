using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public class SlideMovementState : CharacterMovementState
    {
        public SlideMovementState(CharacterModule ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _character.IsSliding = true;
        }
        public override void PhysicsUpdate()
        {
            _character.Slide();
        }
        public override void Exit()
        {
            base.Exit();
            _character.IsSliding = false;
        }
    }
}
