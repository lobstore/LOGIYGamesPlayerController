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
        public LadderExitState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData)
        {
        }
        public override void Enter()
        {
            base.Enter();
            _character.ResetVelocity();

            _character.EventBus.Publish(new LadderExitedEvent
            {
                from = _character.GetComponent<LadderMovementController>().LadderEndpoint.IsTop ? Direction.Up : Direction.Down
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
            _character.GetComponent<CharacterGravityModule>().UseGravity = true;
            _character.IsOnLadder = false;
        }
        public override bool CanEnter()
        {
            return base.CanEnter()
                && _character.GetComponent<LadderMovementController>().LadderEndpoint != null
                && _character.IsOnLadder
                && f();
        }
        bool f()
        {
            return (_character.GetComponent<LadderMovementController>().LadderEndpoint.IsTop && _character.Input.MovementInput.y > 0) || (!_character.GetComponent<LadderMovementController>().LadderEndpoint.IsTop && _character.Input.MovementInput.y < 0);
        }
    }
    public class LadderEnterState : TimedMovementState
    {
        public LadderEnterState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _character.ResetVelocity();
            _character.transform.SetParent(_character.GetComponent<LadderMovementController>().LadderEndpoint.Ladder, true);

            _character.MovementStrategy = new LadderMovement(_character);
            _character.RotationStrategy = new LadderRotation(_character.GetComponent<LadderMovementController>().LadderEndpoint.Ladder);

            _character.GetComponent<CharacterGravityModule>().UseGravity = false;

            _character.transform.localPosition = new Vector3(0, _character.transform.position.y, 0);
            _character.transform.localRotation = Quaternion.identity;

            _character.EventBus.Publish(new LadderEnteredEvent
            {
                from = _character.GetComponent<LadderMovementController>().LadderEndpoint.IsTop ? Direction.Up : Direction.Down
            });
            _character.IsOnLadder = true;
        }
        public override bool CanEnter()
        {
            return base.CanEnter()
                && _character.GetComponent<LadderMovementController>().LadderEndpoint
                && !_character.IsOnLadder;
        }
    }
}