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

        private Transform _mantleTargetTransform;
        private Vector3 _localMantlePoint;

        private float checkDistance = 1f;

        private HandsIK mantleIKController;

        private float lastObstacleHeight;
        private Vector3 lastObstaclePosition;

        #endregion

        #region Hand IK Cache

        private Vector3 _leftHandLocalPoint;
        private Vector3 _rightHandLocalPoint;

        private Vector3 _leftHandNormal;
        private Vector3 _rightHandNormal;

        #endregion

        #region Constructor

        public MantlingMovementState(
            Character ctx,
            MantlingMovmentStateData stateData)
            : base(ctx, stateData)
        {
            checkDistance = stateData.CheckDistance;
            mantleIKController = _character.GetComponent<HandsIK>();
        }

        #endregion

        #region State Lifecycle

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

            UpdateMantleTargetPosition();
            UpdateHandTargets();

            _controller.IsNoClip = true;
        }

        public override void Exit()
        {
            base.Exit();

            DisableHandIK();
            EnableFootIK();

            UpdateMantleTargetPosition();

            _character.transform.position = _mantleTargetPosition;

            _controller.UseGravity = true;
            _controller.IsNoClip = false;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            UpdateMantleTargetPosition();
            UpdateHandTargets();

            _controller.transform.position =
                Vector3.Lerp(
                    _mantleStartPosition,
                    _mantleTargetPosition,
                    DurationTimerProgress);
        }

        public override bool CanEnter()
        {
            float obstacleHeight =
                CalculateObstacleHeight(checkDistance);

            return base.CanEnter() &&
                   obstacleHeight > _controller.MaxStepHeight &&
                   obstacleHeight < 1.6f;
        }

        #endregion

        #region Mantle Detection

        public float CalculateObstacleHeight(float forwardDistance)
        {
            Vector3 direction =
                _controller.transform.forward;

            Vector3 p1 =
                _character.transform.position +
                Vector3.up * _controller.MaxStepHeight;

            Vector3 p2 =
                p1 +
                Vector3.up * _controller.Height -
                Vector3.up * _controller.MaxStepHeight;

            if (Physics.CapsuleCast(
                    p1,
                    p2,
                    _controller.Radius,
                    direction,
                    out RaycastHit forwardHit,
                    forwardDistance))
            {
                return ProcessForwardHit(forwardHit);
            }

            return 0f;
        }

        private float ProcessForwardHit(RaycastHit forwardHit)
        {
            Vector3 topCheckOrigin =
                forwardHit.point +
                _controller.transform.forward * 0.1f +
                Vector3.up * _controller.Height;

            float downDistance =
                _controller.Height + 0.5f;

            if (Physics.Raycast(
                    topCheckOrigin,
                    Vector3.down,
                    out RaycastHit downHit,
                    downDistance))
            {
                CacheMantleData(downHit);

                CalculateHandTargets(downHit);

                return Mathf.Max(0f, lastObstacleHeight);
            }

            return 0f;
        }

        private void CacheMantleData(RaycastHit downHit)
        {
            lastObstacleHeight =
                downHit.point.y -
                _controller.transform.position.y;

            lastObstaclePosition = downHit.point;

            _mantleTargetTransform =
                downHit.collider.transform;

            if (_mantleTargetTransform != null)
            {
                _localMantlePoint =
                    _mantleTargetTransform.InverseTransformPoint(
                        downHit.point);
            }
        }

        #endregion

        #region Mantle Movement

        private void UpdateMantleTargetPosition()
        {
            if (_mantleTargetTransform != null)
            {
                _mantleTargetPosition =
                    _mantleTargetTransform.TransformPoint(
                        _localMantlePoint);
            }
            else
            {
                _mantleTargetPosition =
                    lastObstaclePosition;
            }
        }

        private MantlingType ChooseMantlingType()
        {
            if (lastObstacleHeight <= 0.6f)
                return MantlingType.StepOnLow;

            if (lastObstacleHeight <= 0.7f)
                return MantlingType.StepOnHigh;

            if (lastObstacleHeight <= 1f)
                return MantlingType.BracedLow;

            return MantlingType.BracedHigh;
        }

        #endregion

        #region IK

        private void EnableIK(MantlingType mantlingType)
        {
            if (mantleIKController != null &&
                (mantlingType == MantlingType.BracedLow ||
                 mantlingType == MantlingType.BracedHigh))
            {
                mantleIKController.EnableIK();
            }
        }

        private void DisableHandIK()
        {
            var ik =
                _character.GetComponent<HandsIK>();

            if (ik != null)
            {
                ik.DisableIK();
            }
        }

        private void DisableFootIK()
        {
            var footIK =
                _character.GetComponent<FootIK>();

            if (footIK != null)
            {
                footIK.enabled = false;
            }
        }

        private void EnableFootIK()
        {
            var footIK =
                _character.GetComponent<FootIK>();

            if (footIK != null)
            {
                footIK.enabled = true;
            }
        }

        private void CalculateHandTargets(RaycastHit wallHit)
        {
            if (mantleIKController == null)
                return;

            Vector3 ledgePoint = wallHit.point;
            Vector3 normal = wallHit.normal;

            Vector3 rightOffset =
                _character.transform.right * 0.25f;

            Vector3 handHeightOffset =
                Vector3.up * 0.05f;

            Vector3 surfaceOffset =
                normal * 0f;

            Vector3 leftWorldPoint =
                ledgePoint -
                rightOffset +
                handHeightOffset +
                surfaceOffset;

            Vector3 rightWorldPoint =
                ledgePoint +
                rightOffset +
                handHeightOffset +
                surfaceOffset;

            if (_mantleTargetTransform != null)
            {
                _leftHandLocalPoint =
                    _mantleTargetTransform.InverseTransformPoint(
                        leftWorldPoint);

                _rightHandLocalPoint =
                    _mantleTargetTransform.InverseTransformPoint(
                        rightWorldPoint);
            }
            else
            {
                _leftHandLocalPoint =
                    leftWorldPoint;

                _rightHandLocalPoint =
                    rightWorldPoint;
            }

            _leftHandNormal = normal;
            _rightHandNormal = normal;
        }

        private void UpdateHandTargets()
        {
            if (mantleIKController == null)
                return;

            if (_mantleTargetTransform != null)
            {
                mantleIKController.LeftHandPoint =
                    _mantleTargetTransform.TransformPoint(
                        _leftHandLocalPoint);

                mantleIKController.RightHandPoint =
                    _mantleTargetTransform.TransformPoint(
                        _rightHandLocalPoint);
            }
            else
            {
                mantleIKController.LeftHandPoint =
                    _leftHandLocalPoint;

                mantleIKController.RightHandPoint =
                    _rightHandLocalPoint;
            }

            mantleIKController.LeftHandNormal =
                _leftHandNormal;

            mantleIKController.RightHandNormal =
                _rightHandNormal;
        }

        #endregion
    }
}