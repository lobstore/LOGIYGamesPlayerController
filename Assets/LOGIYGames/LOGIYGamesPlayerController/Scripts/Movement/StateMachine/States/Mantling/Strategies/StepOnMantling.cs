using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames
{
    public class StepOnMantling : MantlingStrategy
    {

        private float obstacleHeight;

        public StepOnMantling(Actor chr, MantlingData data) : base(chr, data)
        {
        }

        public override bool CanEnter()
        {
            if (!HasClearPathToRayOrigin())
                return false;

            TargetTopPoint = PerformDetectionTopDown(checkDistance);

            if (TargetTopPoint.collider != null)
                obstacleHeight = CalculateObstacleHeight();
            else
                obstacleHeight = 0;

            if (!HasEnoughSpace())
                return false;

            return obstacleHeight > _controller.MaxStepHeight &&
                   obstacleHeight <= _controller.Height*0.5f;
        }

        public override bool CanExit()
        {
            return Duration.IsFinished;
        }
        public override void Enter()
        {
            MantlingType = ChooseMantlingType();
            Duration.Start();
            MantleStartPosition = _characterModule.transform.position;

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
            return TargetTopPoint.point.y - _characterModule.transform.position.y;
        }

        private Vector3 GetTopDownRayOrigin(float forwardDistance)
        {
            return _characterModule.transform.position +
                   _characterModule.transform.forward * (_controller.Radius + forwardDistance) +
                   _characterModule.transform.up * (_controller.Height*0.5f);
        }
        private bool HasClearPathToRayOrigin()
        {
            Vector3 rayOrigin = GetTopDownRayOrigin(checkDistance);

            float radius = _controller.Radius;
            float height = _controller.Height;

            float yOffset =
                rayOrigin.y - _controller.transform.position.y;

            Vector3 bottom =
                _controller.transform.position +
                _controller.transform.up * (radius + yOffset);

            Vector3 top =
                bottom +
               _controller.transform.up * (height - radius * 2f);

            Debug.DrawLine(bottom, top, Color.yellow);
            Debug.DrawRay(
                (bottom + top) * 0.5f,
               _controller.transform.forward * checkDistance,
                Color.cyan);

            return !Physics.CapsuleCast(
                bottom,
                top,
                radius,
                _controller.transform.forward,
                out _,
                checkDistance,
                mantlingLayers,
                QueryTriggerInteraction.Ignore);
        }
        private bool HasEnoughSpace()
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
                -_controller.transform.up * (_controller.Height * 0.49f),
                Color.red);

            if (Physics.Raycast(
                origin,
                -_controller.transform.up,
                out RaycastHit hit,
                _controller.Height *0.49f,
                mantlingLayers))
            {
                return hit;
            }

            return default;
        }

        private MantlingType ChooseMantlingType()
        {
            if (obstacleHeight <= _controller.Height * 0.4f)
                return MantlingType.StepOnLow;

            return MantlingType.StepOnHigh;
        }
    }
}
