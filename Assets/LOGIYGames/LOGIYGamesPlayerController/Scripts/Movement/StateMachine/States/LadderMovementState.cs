using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public class LadderMovementState : BaseMovementState
    {
        LadderMovementController ladderMovementController;
        public LadderMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            ladderMovementController = _character.GetComponent<LadderMovementController>();
        }
        float t;
        public override void Enter()
        {
            base.Enter();
            t = 0.01f;
            _character.RotationStrategy = new LadderClimbRotation(ladderMovementController);
            _character.MovementStrategy = new NoneMovement();
            _controller .UseGravity = false;
            _character.IsOnLadder = true;
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            t += _character.Input.MovementInput.y * _character.SpeedMultiplier * Time.deltaTime;
            ladderMovementController.t = Mathf.Clamp01(t);
            ladderMovementController.Climb();
        }
        public override void Exit()
        {
            base.Exit();
            _controller.UseGravity = true;
            _character.IsOnLadder =false;
            _character.RotationStrategy = _character.DefaultRotationStrategy;
            _character.MovementStrategy = _character.DefaultMovementStrategy;
        }
    }
}