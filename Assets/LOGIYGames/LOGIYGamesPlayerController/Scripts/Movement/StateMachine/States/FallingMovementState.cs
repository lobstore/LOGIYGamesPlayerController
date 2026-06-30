using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames.Movement
{
    public class FallingMovementState : CharacterMovementState
    {
        public FallingMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData) { }

        public override void Enter()
        {

            base.Enter();
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();

        }
        public override void Exit()
        {
            base.Exit();
        }
    }

}
