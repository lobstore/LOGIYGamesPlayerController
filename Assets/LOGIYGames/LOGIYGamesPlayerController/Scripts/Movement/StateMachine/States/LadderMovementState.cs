using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using UnityEngine;

namespace LOGIYGames
{
    public abstract class LadderMovementState : BaseMovementState
    {
        public LadderMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {

        }
    }
    public class LadderUpState : LadderMovementState
    {
        public LadderUpState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
        protected override void Aim()
        {

        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }
    }
    public class LadderDownState : LadderMovementState
    {
        public LadderDownState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
        protected override void Aim()
        {

        }
    }
    public class LadderIdleState : BaseMovementState
    {
        public LadderIdleState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            Debug.Log(_character.MovementStrategy);
        }
        protected override void Aim()
        {

        }
    }
    public class LadderExitState : TimedMovementState
    {
        public LadderExitState(Character ctx, TimedMovementStateData stateData) : base(ctx, stateData)
        {
        }
        public override void Exit()
        {
            base.Exit();
            _character.RotationStrategy = _character.DefaultRotationStrategy;
            _character.MovementStrategy = new CameraRelativeMovement(_character);
            _character.GetComponent<CharacterGravityModule>().UseGravity = true;
        }
        protected override void Aim()
        {

        }
        public override bool CanEnter()
        {
            return base.CanEnter() && _character.Target != null;
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
            _character.GetComponent<CharacterGravityModule>().UseGravity = false;
            _character.SetPosition(_character.Target.position);
            _character.RotationStrategy = new ToTargetRotation(_character.Target);
            _character.MovementStrategy = new YAxisMovement(_character);
        }
        protected override void Aim()
        {
            
        }
        public override void Exit()
        {
            base.Exit();
            _character.RotateToDirection(_character.Target.forward);

        }
        public override bool CanEnter()
        {
            return base.CanEnter()&& _character.Target!=null;
        }
    }
}