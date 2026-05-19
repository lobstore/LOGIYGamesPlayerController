using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using UnityEngine;

namespace LOGIYGames
{
    public class MantlingMovementState : TimedMovementState
    {
        private Vector3 _mantleTargetPosition;
        private Vector3 _mantleStartPosition;


        public MantlingMovementState(
            Character ctx,
            TimedMovementStateData stateData)
            : base(ctx, stateData) { }

        float lastObstacleHeight;
        Vector3 lastObstaclePosition;

        public override void Enter()
        {
            base.Enter();
            _controller.UseGravity = false;
            _character.EventBus.Publish(new MantlingEvent
            {
                ObstacleHeight = lastObstacleHeight
            });

            _character.RotationStrategy = new NoneRotation(_character);
            _character.MovementStrategy = new NoneMovement();
            _mantleStartPosition = _character.transform.position;
            _mantleTargetPosition = lastObstaclePosition;
            _controller.IsNoClip = true;
        }
        public override void Exit()
        {
            base.Exit();
            _character.transform.position = _mantleTargetPosition;
            _controller.UseGravity = true;
            _controller.IsNoClip = false;
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            Debug.Log(DurationTimerProgress);
            _controller.transform.position = Vector3.Lerp(_mantleStartPosition, _mantleTargetPosition, DurationTimerProgress);
        }

        public override bool CanEnter()
        {
            return base.CanEnter() &&
                   CalculateObtacleHeight() > _controller.MaxStepHeight && CalculateObtacleHeight() < 1.6f;
        }

        private float CalculateObtacleHeight()
        {
            Vector3 origin = _controller.transform.position +
                             Vector3.up * (_controller.MaxStepHeight + 0.05f);

            Vector3 direction = _controller.transform.forward;
            float forwardDistance = 0.1f;
            Vector3 p1 = _character.transform.position + Vector3.up * _controller.MaxStepHeight;
            Vector3 p2 = p1 + Vector3.up * _controller.Height - Vector3.up * _controller.MaxStepHeight;

            Debug.DrawRay(
                p2,
                direction * 2,
                Color.red,
                2f);
            Debug.DrawRay(
    p1,
    direction * 2,
    Color.red,
    2f);
            if (Physics.CapsuleCast(
                    p1,
                    p2,
                    _controller.Radius,
                    direction,
                    out RaycastHit forwardHit,
                    forwardDistance))
            {
                Vector3 topCheckOrigin =
                    forwardHit.point + _controller.transform.forward *0.1f +
                    Vector3.up * _controller.Height;

                float downDistance = _controller.Height + 0.5f;

                Debug.DrawRay(
                    topCheckOrigin,
                    Vector3.down * downDistance,
                    Color.green,
                    0.1f);

                if (Physics.Raycast(
                        topCheckOrigin,
                        Vector3.down,
                        out RaycastHit downHit,
                        downDistance))
                {
                    float obstacleHeight =
                        downHit.point.y - _controller.transform.position.y;

                    Debug.DrawLine(
                        forwardHit.point,
                        downHit.point,
                        Color.yellow,
                        0.1f);
                    lastObstaclePosition = downHit.point;
                    lastObstacleHeight = obstacleHeight;

                    return Mathf.Max(0f, obstacleHeight);
                }
            }

            return 0f;
        }
    }
}
