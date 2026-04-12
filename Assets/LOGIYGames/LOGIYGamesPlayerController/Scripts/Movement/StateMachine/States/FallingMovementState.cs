using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames.Movement
{
    public class FallingMovementState : BaseMovementState
    {
        public FallingMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData) { }

        public override void Enter()
        {

            base.Enter();
            _character.IsFalling = true;
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();

        }
        public override void Exit()
        {
            base.Exit();
            _character.IsFalling = false;
        }
    }

}
