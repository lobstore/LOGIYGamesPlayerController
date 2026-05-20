using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using LOGIYGames.Shared.Character.Events;
using LOGIYGames.Shared.Enums;
using RealStep;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace LOGIYGames
{
    public class MantlingMovementState : TimedMovementState
    {
        private Vector3 _mantleTargetPosition;
        private Vector3 _mantleStartPosition;

        // IK
        private Vector3 _leftHandPoint;
        private Vector3 _rightHandPoint;

        private Vector3 _leftHandNormal;
        private Vector3 _rightHandNormal;

        public Vector3 LeftHandPoint => _leftHandPoint;
        public Vector3 RightHandPoint => _rightHandPoint;

        public Vector3 LeftHandNormal => _leftHandNormal;
        public Vector3 RightHandNormal => _rightHandNormal;

        public bool HasValidHandTargets { get; private set; }
        public MantlingMovementState(
            Character ctx,
            TimedMovementStateData stateData)
            : base(ctx, stateData)
        {
        }

        float lastObstacleHeight;
        Vector3 lastObstaclePosition;

        public override void Enter()
        {
            base.Enter();
            MantlingType mantlingType = ChooseMantlingType();
            var ik = _character.GetComponent<MantleIKController>();
            if (ik != null && (mantlingType == MantlingType.BracedLow|| mantlingType == MantlingType.BracedHigh))
            {
                ik.EnableIK();
            }
            var footik = _character.GetComponent<FootIK>();
            if (footik != null)
            {
                footik.enabled = false;
            }
            _controller.UseGravity = false;
            _character.EventBus.Publish(new MantlingEvent
            {
                Type = mantlingType
            });

            _character.RotationStrategy = new NoneRotation(_character);
            _character.MovementStrategy = new NoneMovement();
            _mantleStartPosition = _character.transform.position;
            _mantleTargetPosition = lastObstaclePosition;
            _controller.IsNoClip = true;
        }

        private MantlingType ChooseMantlingType()
        {
            MantlingType type = MantlingType.StepOnLow;
            if (lastObstacleHeight <= 0.6f)
            {
                type = MantlingType.StepOnLow;
            }
            else if (lastObstacleHeight > 0.6f && lastObstacleHeight <= 0.7f)
            {
                type = MantlingType.StepOnHigh;
            }
            else if (lastObstacleHeight > 0.7f && lastObstacleHeight <= 1f)
            {
                type = MantlingType.BracedLow;
            }
            else if (lastObstacleHeight > 1f && lastObstacleHeight <= 1.6f)
            {
                type = MantlingType.BracedHigh;
            }

            return type;
        }

        public override void Exit()
        {
            base.Exit();
            var ik = _character.GetComponent<MantleIKController>();
            if (ik != null)
            {
                ik.DisableIK();
            }
            var footik = _character.GetComponent<FootIK>();
            if (footik != null)
            {
                footik.enabled = true;
            }
            _character.transform.position = _mantleTargetPosition;
            _controller.UseGravity = true;
            _controller.IsNoClip = false;
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
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

            if (Physics.CapsuleCast(
                    p1,
                    p2,
                    _controller.Radius,
                    direction,
                    out RaycastHit forwardHit,
                    forwardDistance))
            {
                Vector3 topCheckOrigin =
                    forwardHit.point + _controller.transform.forward * 0.1f +
                    Vector3.up * _controller.Height;

                float downDistance = _controller.Height + 0.5f;

                if (Physics.Raycast(
                        topCheckOrigin,
                        Vector3.down,
                        out RaycastHit downHit,
                        downDistance))
                {
                    float obstacleHeight =
                        downHit.point.y - _controller.transform.position.y;

                    lastObstaclePosition = downHit.point;
                    lastObstacleHeight = obstacleHeight;
                    CalculateHandTargets(downHit);
                    return Mathf.Max(0f, obstacleHeight);
                }
            }

            return 0f;
        }
        private void CalculateHandTargets(RaycastHit wallHit)
        {
            Vector3 ledgePoint = wallHit.point;
            Vector3 normal = wallHit.normal;

            Vector3 rightOffset = _character.transform.right * 0.25f;
            Vector3 handHeightOffset = Vector3.up * 0.05f;
            Vector3 surfaceOffset = normal * 0;

            _character.LeftHandPoint =
                ledgePoint - rightOffset + handHeightOffset + surfaceOffset;

            _character.RightHandPoint=
                ledgePoint + rightOffset + handHeightOffset + surfaceOffset;

            _character.LeftHandNormal= normal;
            _character.RightHandNormal = normal;

            HasValidHandTargets = true;
        }
    }
}
