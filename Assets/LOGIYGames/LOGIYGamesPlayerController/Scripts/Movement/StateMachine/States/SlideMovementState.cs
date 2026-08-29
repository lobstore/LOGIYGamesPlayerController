using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public class SlideMovementState : MovementStateBase
    {
        public SlideMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }
        public override void PhysicsUpdate()
        {
            //_character.Slide();
        }
        public override void Exit()
        {
            base.Exit();
        }
    }
}
