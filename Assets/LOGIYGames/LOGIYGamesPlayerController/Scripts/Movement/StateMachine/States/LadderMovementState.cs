using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames
{
    public class LadderUpState : BaseMovementState
    {
        public LadderUpState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
    }
    public class LadderDownState : BaseMovementState
    {
        public LadderDownState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
    }
    public class LadderIdleState : BaseMovementState
    {
        public LadderIdleState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
    }
    public class LadderExitState : TimedMovementState
    {
        ControllerWrapperBase controller;
        LadderMovementController ladderMovement;
        public LadderExitState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData)
        {
            ladderMovement = _character.GetComponent<LadderMovementController>();
            controller = ctx.GetComponent<ControllerWrapperBase>();
        }
        public override void Enter()
        {
            base.Enter();
            _character.ResetVelocity();

            _character.EventBus.Publish(new LadderExitedEvent
            {
            });

        }
        protected override void Move()
        {

        }
        protected override void Rotate()
        {

        }
        public override void Exit()
        {
            base.Exit();
            _character.MovementStrategy = _character.DefaultMovementStrategy;
            _character.RotationStrategy = _character.DefaultRotationStrategy;
            _character.transform.SetParent(null);
            _character.IsOnLadder = false;
            controller.UseGravity = true;

        }
        public override bool CanEnter()
        {
            return base.CanEnter()
                && _character.IsOnLadder
                &&f();
        }
        bool f()
        {
            return (_character.Input.MovementInput.y > 0&& !ladderMovement.LadderInFrontLegs)|| (_character.Input.MovementInput.y < 0 && _character.IsGrounded);
        }
    }
    public class LadderEnterState : TimedMovementState
    {
        ControllerWrapperBase controller;
        LadderMovementController ladderMovement;
        public LadderEnterState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData)
        {
            ladderMovement = _character.GetComponent<LadderMovementController>();
            controller = ctx.GetComponent<ControllerWrapperBase>();
        }

        public override void Enter()
        {
            base.Enter();
            _character.ResetVelocity();

            _character.MovementStrategy = new LadderMovement(_character);
            _character.RotationStrategy = new LadderRotation(ladderMovement.Ladder.transform);

            controller.UseGravity = false;

            _character.transform.position = new Vector3(ladderMovement.Ladder.transform.position.x, _character.transform.position.y, ladderMovement.Ladder.transform.position.z) - ladderMovement.Ladder.transform.forward*0.5f;
            _character.transform.rotation = ladderMovement.Ladder.transform.rotation;
            _character.transform.SetParent(ladderMovement.Ladder.transform, true);

            _character.EventBus.Publish(new LadderEnteredEvent
            {
            });
            _character.IsOnLadder = true;
        }
        public override bool CanEnter()
        {
            return base.CanEnter()
                && !_character.IsOnLadder
                && ladderMovement.LadderInFrontLegs;
        }
    }
}