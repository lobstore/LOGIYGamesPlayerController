using LOGIYGames.CharacterCore;
using UnityEngine;
namespace LOGIYGames.Movement
{
    public class WallRunMovementState : BaseMovementState
    {
        Vector3 normal;
        public WallRunMovementState(Character ctx, MovementStateData stateData) : base(ctx, stateData)
        {
        }
        public override void Enter()
        {
            base.Enter();
            _character.RotationStrategy = new ToMoveDirectionRotation(_character);
            _character.MovementStrategy = new WallRunMovement(_character);
            _controller.UseGravity = false;
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            normal = _character.Sensors.IsObstacleLegsRight ? _character.Sensors.LegsRightHit.normal : _character.Sensors.LegsLeftHit.normal;
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _controller.ForceMove(-normal);
        }
        public override void Exit()
        {
            base.Exit();
            _controller.UseGravity = true;
        }
    }
    //public class WallrunState : ContinuousState
    //{
    //    public WallrunState(MovementStateDriver ctx, StateData stateData) : base(ctx, stateData)
    //    {
    //        wallRunGravityMultiplier = 0;
    //        useWallCliping = true;
    //    }
    //    private float wallRunGravityMultiplier;
    //    private bool useWallCliping;
    //    Vector3 normal;
    //    Vector3 magnit => -normal;

    //    public override bool CanBeExecuted()
    //    {
    //        return base.CanBeExecuted()
    //                    && (Sensors.IsObstacleLegsLeft || Sensors.IsObstacleLegsRight)
    //                    && !Sensors.IsGrounded
    //                    && MovementInput.y > 0
    //                    && !Sensors.IsObstacleLegsFront
    //                    && Vector3.Angle(Character.transform.forward, Camera.main.transform.forward) < 60;
    //    }

    //    private void Magnit()
    //    {
    //        CController.Move(magnit * Time.deltaTime);

    //    }
    //    protected override void Rotate()
    //    {
    //        Character.RotateToDirection(moveDirection, TurnSmoothTime);
    //    }
    //    public override void Enter()
    //    {
    //        base.Enter();
    //        CharacterGravity.Velocity = Vector3.zero;
    //        CharacterGravity.UseGravity = false;
    //    }
    //    public override void Exit()
    //    {
    //        base.Exit();
    //        CharacterGravity.UseGravity = true;
    //    }
    //}
}
