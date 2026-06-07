using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Enums;
using UnityEngine;

namespace LOGIYGames
{
    public class StepOnMantling : MantlingStrategy
    {

        private float obstacleHeight;

        public StepOnMantling(CharacterModule chr, MantlingData data) : base(chr, data)
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

            return obstacleHeight > _characterModule.MaxStepHeight &&
                   obstacleHeight <= _characterModule.Height * 0.5;
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
            _characterModule.transform.position = MantleTargetPosition;
        }
        public override void Update()
        {
            if (MantleTargetTransform != null)
            {
                MantleTargetPosition =
                    MantleTargetTransform.TransformPoint(MantleTargetLocalPoint);
            }

            _characterModule.transform.position =
                Vector3.Lerp(
                    MantleStartPosition,
                    MantleTargetPosition,
                    Duration.Progress);
        }

        private float CalculateObstacleHeight()
        {
            return TargetTopPoint.point.y - _characterModule.transform.position.y;
        }

        private Vector3 GetTopDownRayOrigin(float forwardDistance)
        {
            return _characterModule.transform.position +
                   _characterModule.transform.forward * (_characterModule.Radius + forwardDistance) +
                   _characterModule.transform.up * (_characterModule.Height * 0.5f);
        }
        //private bool HasClearPathToRayOrigin()
        //{
        //    Vector3 rayOrigin = GetTopDownRayOrigin(checkDistance);

        //    Vector3 start =
        //        _characterModule.transform.position +
        //        _characterModule.transform.up * (_characterModule.Height);

        //    Vector3 direction = rayOrigin - start;
        //    float distance = direction.magnitude;

        //    Debug.DrawLine(start, rayOrigin, Color.yellow);

        //    return !Physics.Raycast(
        //        start,
        //        direction.normalized,
        //        out _,
        //        distance,
        //        mantlingLayers,
        //        QueryTriggerInteraction.Ignore);
        //}
        private bool HasClearPathToRayOrigin()
        {
            Vector3 rayOrigin = GetTopDownRayOrigin(checkDistance);

            float radius = _characterModule.Radius;
            float height = _characterModule.Height;

            float yOffset =
                rayOrigin.y - _characterModule.transform.position.y;

            Vector3 bottom =
                _characterModule.transform.position +
                _characterModule.transform.up * (radius + yOffset);

            Vector3 top =
                bottom +
                _characterModule.transform.up * (height - radius * 2f);

            Debug.DrawLine(bottom, top, Color.yellow);
            Debug.DrawRay(
                (bottom + top) * 0.5f,
                _characterModule.transform.forward * checkDistance,
                Color.cyan);

            return !Physics.CapsuleCast(
                bottom,
                top,
                radius,
                _characterModule.transform.forward,
                out _,
                checkDistance,
                mantlingLayers,
                QueryTriggerInteraction.Ignore);
        }
        private bool HasEnoughSpace()
        {
            float radius = _characterModule.Radius;
            float height = _characterModule.Height;

            Vector3 bottom =
                TargetTopPoint.point +
                _characterModule.transform.forward * radius +
                _characterModule.transform.up * (radius + 0.1f);

            Vector3 top =
                bottom + _characterModule.transform.up * (height - radius * 2 - 0.1f);

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
                -_characterModule.transform.up * (_characterModule.Height + 0.3f),
                Color.red);

            if (Physics.Raycast(
                origin,
                -_characterModule.transform.up,
                out RaycastHit hit,
                _characterModule.Height + 0.3f,
                mantlingLayers))
            {
                return hit;
            }

            return default;
        }

        private MantlingType ChooseMantlingType()
        {
            if (obstacleHeight <= _characterModule.Height * 0.4f)
                return MantlingType.StepOnLow;

            return MantlingType.StepOnHigh;
        }
    }
}
