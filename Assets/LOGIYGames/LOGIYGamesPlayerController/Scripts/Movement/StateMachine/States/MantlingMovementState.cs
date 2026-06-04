using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using RealStep;
using UnityEngine;

namespace LOGIYGames
{
    public class MantlingMovementState : TimedMovementState
    {
        #region Fields

        private Vector3 _mantleTargetPosition;
        private Vector3 _mantleStartPosition;

        private float checkDistance = 1f;

        private HandsIK mantleIKController;

        private float obstacleHeight;
        private RaycastHit obstacleTopPoint;

        private Transform _mantleTargetTransform;
        private Vector3 _mantleTargetLocalPoint;
        LayerMask mantlingLayers;
        #endregion

        #region Hand IK Cache

        private Vector3 _leftHandLocalPoint;
        private Vector3 _rightHandLocalPoint;

        private Vector3 _leftHandNormal;
        private Vector3 _rightHandNormal;

        private Vector3 _ledgePoint;
        private Vector3 _ledgeNormal;

        #endregion

        #region Constructor

        public MantlingMovementState(
            CharacterModule ctx,
            MantlingMovmentStateData stateData)
            : base(ctx, stateData)
        {
            checkDistance = stateData.CheckDistance;
            mantleIKController = _character.GetComponent<HandsIK>();
            mantlingLayers = stateData.IncludeLayers;
        }

        #endregion

        #region Lifecycle

        public override void Enter()
        {
            base.Enter();

            MantlingType mantlingType = ChooseMantlingType();

            EnableIK(mantlingType);
            DisableFootIK();

            _controller.UseGravity = false;

            _character.EventBus.Publish(new MantlingEvent
            {
                Type = mantlingType
            });

            _character.RotationStrategy = new NoneRotation(_character);
            _character.MovementStrategy = new NoneMovement();

            _mantleStartPosition = _character.transform.position;

            _mantleTargetTransform = obstacleTopPoint.collider != null
                ? obstacleTopPoint.collider.transform
                : null;

            if (_mantleTargetTransform != null)
            {
                _mantleTargetLocalPoint =
                    _mantleTargetTransform.InverseTransformPoint(obstacleTopPoint.point);

                _mantleTargetPosition = obstacleTopPoint.point;
            }
            else
            {
                _mantleTargetPosition = obstacleTopPoint.point;
            }

            _ledgePoint = obstacleTopPoint.point;
            _ledgeNormal = obstacleTopPoint.normal;

            CalculateHandTargets(_ledgePoint, _ledgeNormal);
            UpdateHandTargets();

            _controller.IsNoClip = true;
        }

        public override void Exit()
        {
            base.Exit();

            DisableHandIK();
            EnableFootIK();

            _character.transform.position = _mantleTargetPosition;

            _controller.UseGravity = true;
            _controller.IsNoClip = false;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (_mantleTargetTransform != null)
            {
                _mantleTargetPosition =
                    _mantleTargetTransform.TransformPoint(_mantleTargetLocalPoint);
            }

            _controller.transform.position =
                Vector3.Lerp(
                    _mantleStartPosition,
                    _mantleTargetPosition,
                    DurationTimerProgress);

            UpdateDynamicHandTargets();
        }

        #endregion

        #region Mantle Detection

        public override bool CanEnter()
        {
            if (!HasClearPathToRayOrigin())
                return false;

            obstacleTopPoint = PerformDetectionTopDown(checkDistance);

            if (obstacleTopPoint.collider != null)
                obstacleHeight = CalculateObstacleHeight();
            else
                obstacleHeight = 0;

            if (!HasEnoughSpaceAhead())
                return false;

            return base.CanEnter() &&
                   obstacleHeight > _controller.MaxStepHeight &&
                   obstacleHeight <= _controller.Height + 0.3f;
        }

        private float CalculateObstacleHeight()
        {
            return obstacleTopPoint.point.y - _controller.transform.position.y;
        }

        private Vector3 GetTopDownRayOrigin(float forwardDistance)
        {
            return _controller.transform.position +
                   _controller.transform.forward * (_controller.Radius + forwardDistance) +
                   _controller.transform.up * (_controller.Height + 0.3f);
        }

        /// <summary>
        /// Проверяет, что до точки, из которой будет выполняться raycast вниз,
        /// нет препятствий.
        /// </summary>
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
                obstacleTopPoint.point +
                _controller.transform.forward * radius +
                _controller.transform.up * (radius + 0.1f);

            Vector3 top =
                bottom + _controller.transform.up * (height - radius * 2 - 0.1f);

            DrawCapsule(bottom, top, radius, Color.green);

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

        #endregion
        private void DrawCapsule(Vector3 start, Vector3 end, float radius, Color color)
        {
            Vector3 axis = (end - start).normalized;

            Vector3 bottomPoint = start - axis * radius;
            Vector3 topPoint = end + axis * radius;

            // Центральная линия через всю капсулу
            Debug.DrawLine(bottomPoint, topPoint, color);

            // Боковые линии оставляем как были
            Debug.DrawLine(start + Vector3.right * radius,
                           end + Vector3.right * radius, color);

            Debug.DrawLine(start - Vector3.right * radius,
                           end - Vector3.right * radius, color);

            Debug.DrawLine(start + Vector3.forward * radius,
                           end + Vector3.forward * radius, color);

            Debug.DrawLine(start - Vector3.forward * radius,
                           end - Vector3.forward * radius, color);
        }
        #region Hands

        private void CalculateHandTargets(Vector3 ledgePoint, Vector3 normal)
        {
            if (mantleIKController == null)
                return;

            Vector3 rightOffset = _character.transform.right * 0.25f;
            Vector3 handOffset = Vector3.up * 0.05f;

            _leftHandLocalPoint = (ledgePoint - rightOffset + handOffset);
            _rightHandLocalPoint = (ledgePoint + rightOffset + handOffset);

            _leftHandNormal = normal;
            _rightHandNormal = normal;
        }
        private void UpdateDynamicHandTargets()
        {
            if (mantleIKController == null)
                return;

            if (_mantleTargetTransform != null)
            {
                _ledgePoint =
                    _mantleTargetTransform.TransformPoint(_mantleTargetLocalPoint);
            }

            Vector3 rightOffset = _character.transform.right * 0.25f;
            Vector3 handOffset = Vector3.up * 0.05f;

            Vector3 leftWorld =
                _ledgePoint - rightOffset + handOffset;

            Vector3 rightWorld =
                _ledgePoint + rightOffset + handOffset;

            mantleIKController.LeftHandPoint = leftWorld;
            mantleIKController.RightHandPoint = rightWorld;

            mantleIKController.LeftHandNormal = _ledgeNormal;
            mantleIKController.RightHandNormal = _ledgeNormal;
        }

        private void UpdateHandTargets()
        {
            if (mantleIKController == null)
                return;

            mantleIKController.LeftHandPoint = _leftHandLocalPoint;
            mantleIKController.RightHandPoint = _rightHandLocalPoint;

            mantleIKController.LeftHandNormal = _leftHandNormal;
            mantleIKController.RightHandNormal = _rightHandNormal;
        }

        #endregion

        #region IK / Movement helpers

        private void EnableIK(MantlingType type)
        {
            if (mantleIKController != null &&
                (type == MantlingType.BracedLow ||
                 type == MantlingType.BracedHigh))
            {
                mantleIKController.EnableIK();
            }
        }

        private void DisableHandIK()
        {
            var ik = _character.GetComponent<HandsIK>();
            if (ik != null) ik.DisableIK();
        }

        private void DisableFootIK()
        {
            var footIK = _character.GetComponent<FootIK>();
            if (footIK != null) footIK.enabled = false;
        }

        private void EnableFootIK()
        {
            var footIK = _character.GetComponent<FootIK>();
            if (footIK != null) footIK.enabled = true;
        }

        private MantlingType ChooseMantlingType()
        {
            if (obstacleHeight <= _character.Height * 0.2f)
                return MantlingType.StepOnLow;

            if (obstacleHeight <= _character.Height * 0.4f)
                return MantlingType.StepOnHigh;

            if (obstacleHeight <= _character.Height * 0.6f)
                return MantlingType.BracedLow;

            return MantlingType.BracedHigh;
        }

        #endregion
    }
}