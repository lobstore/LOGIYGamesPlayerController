using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace LOGIYGames
{
    public class LadderMovementState : MovementStateBase
    {
        LadderClimbController ladderMovementController;
        public LadderMovementState(Actor ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            ladderMovementController = _character.GetComponent<LadderClimbController>();
        }

        public override void Enter()
        {
            base.Enter();
            ladderMovementController.Enter();
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (!Data.IsAnimationDrivenMovement ) 
            ladderMovementController.Climb();
            ladderMovementController.Rotate();
        }
        public override void Exit()
        {
            base.Exit();
            ladderMovementController.Exit();
        }
        public override bool CanExit()
        {
            return ladderMovementController.CanExit();
        }
        public override bool CanEnter()
        {
            return ladderMovementController.CanEnter();
        }
    }
}