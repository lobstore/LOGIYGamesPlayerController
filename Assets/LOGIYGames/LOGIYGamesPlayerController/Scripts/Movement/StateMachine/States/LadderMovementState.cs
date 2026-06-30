using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public class LadderMovementState : CharacterMovementState
    {
        LadderMovementController ladderMovementController;
        public LadderMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
            ladderMovementController = _character.GetComponent<LadderMovementController>();
        }
        float t;
        float distanceTravelled;
        public override void Enter()
        {
            base.Enter();
            distanceTravelled = 0.01f;
            _character.RotationStrategy = new LadderClimbRotation(ladderMovementController);
            _character.MovementStrategy = new NoneMovement();
            _controller.UseGravity = false;
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            distanceTravelled += _character.Speed * _character.Input.MovementInput.y * Time.deltaTime;
            t = distanceTravelled / ladderMovementController.Ladder.Lenght;
            //t += _character.Input.MovementInput.y * _character.SpeedMultiplier * Time.deltaTime;
            ladderMovementController.t = Mathf.Clamp01(t);
            ladderMovementController.Climb();
        }
        public override void Exit()
        {
            base.Exit();
            _controller.UseGravity = true;
            _character.RotationStrategy = _character.DefaultRotationStrategy;
            _character.MovementStrategy = _character.DefaultMovementStrategy;
        }
    }
}