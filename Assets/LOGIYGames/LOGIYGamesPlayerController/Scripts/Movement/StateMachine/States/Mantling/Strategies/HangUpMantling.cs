using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames
{
    public class HangUpMantling : MantlingStrategy
    {
        private float obstacleHeight;
        public HangUpMantling(Actor chr, MantlingData data) : base(chr, data) { }
        public override bool CanEnter()
        {
            if (!HasClearPathToRayOrigin())
                return false;

            TargetTopPoint = PerformDetectionTopDown(checkDistance);

            if (TargetTopPoint.collider != null)
                obstacleHeight = CalculateObstacleHeight();
            else
                obstacleHeight = 0;

            if (!HasEnoughSpaceAhead())
                return false;

            return obstacleHeight > _controller.Height * 0.5 &&
                   obstacleHeight <= _controller.Height + 0.3f;
        }

        public override bool CanExit()
        {
            return Duration.IsFinished;
        }

        public override void Enter()
        {
            MantlingType = ChooseMantlingType();
            Duration.Start();
            MantleStartPosition = _controller.transform.position;

            MantleTargetTransform = TargetTopPoint.collider != null
                ? TargetTopPoint.collider.transform
                : null;

            if (MantleTargetTransform != null)
            {
                MantleTargetLocalPoint =
                    MantleTargetTransform.InverseTransformPoint(TargetTopPoint.point);

                MantleTargetPosition = TargetTopPoint.point;
            }
            else
            {
                MantleTargetPosition = TargetTopPoint.point;
            }
        }
        public override void Exit()
        {
            _controller.SetPosition(MantleTargetPosition);
        }
        public override void Tick()
        {
            if (MantleTargetTransform != null)
            {
                MantleTargetPosition =
                    MantleTargetTransform.TransformPoint(MantleTargetLocalPoint);
            }
            var target = Vector3.Lerp(
                     MantleStartPosition,
                     MantleTargetPosition,
                     Duration.Progress);
            _controller.SetPosition(target);

        }

        private float CalculateObstacleHeight()
        {
            return TargetTopPoint.point.y - _controller.transform.position.y;
        }

        private Vector3 GetTopDownRayOrigin(float forwardDistance)
        {
            return _controller.transform.position +
                   _controller.transform.forward * (_controller.Radius + forwardDistance) +
                   _controller.transform.up * (_controller.Height + 0.3f);
        }
        private bool HasClearPathToRayOrigin()
        {
            Vector3 rayOrigin = GetTopDownRayOrigin(checkDistance);

            Vector3 start =
                _controller.transform.position +
                _controller.transform.up * (_controller.Height);

            Vector3 direction = rayOrigin - start;
            float distance = direction.magnitude;

            Debug.DrawLine(start, rayOrigin, Color.yellow);

            return !Physics.Raycast(
                start,
                direction.normalized,
                out _,
                distance,
                mantlingLayers,
                QueryTriggerInteraction.Ignore);
        }

        private bool HasEnoughSpaceAhead()
        {
            float radius = _controller.Radius;
            float height = _controller.Height;

            Vector3 bottom =
                TargetTopPoint.point +
                _controller.transform.forward * radius +
                _controller.transform.up * (radius + 0.1f);

            Vector3 top =
                bottom + _controller.transform.up * (height - radius * 2 - 0.1f);

            return !Physics.CheckCapsule(
                bottom,
                top,
                radius,
                mantlingLayers,
                QueryTriggerInteraction.Ignore);
        }

        private RaycastHit PerformDetectionTopDown(float forwardDistance)
        {
            Vector3 origin = GetTopDownRayOrigin(forwardDistance);

            Debug.DrawRay(
                origin,
                -_controller.transform.up * (_controller.Height + 0.3f),
                Color.red);

            if (Physics.Raycast(
                origin,
                -_controller.transform.up,
                out RaycastHit hit,
                _controller.Height + 0.3f,
                mantlingLayers))
            {
                return hit;
            }

            return default;
        }

        private MantlingType ChooseMantlingType()
        {
            if (obstacleHeight <= _controller.Height * 0.7f)
                return MantlingType.BracedLow;

            return MantlingType.BracedHigh;
        }
    }
}
