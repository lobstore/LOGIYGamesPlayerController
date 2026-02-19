using UnityEngine;
using UnityEngine.Animations.Rigging;
using LOGIYGames.Timers;

namespace LOGIYGames
{
    public enum HandIKPoint
    {
        RightHand,
        LeftHand,
        BothHands, // выбирается ближайшая
        TwoHands   // обе руки одновременно
    }

    public class IKGrabItem : MonoBehaviour
    {
        [SerializeField] private NearestInteractableFinder nearestInteractableFinder;
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private TwoBoneIKConstraint rightHandIK;
        [SerializeField] private TwoBoneIKConstraint leftHandIK;
        [SerializeField] private Transform rightHandTransform;
        [SerializeField] private Transform leftHandTransform;
        [SerializeField] private Transform chestTransform;
        [SerializeField] private Transform defaultTransformRight;
        [SerializeField] private Transform defaultTransformLeft;
        [SerializeField] private float touchDuration = 1f;
        [SerializeField] private float handLength = 2f;
        [SerializeField] private float lerpSpeed = 10f;
        [SerializeField] private float resetLerpSpeed = 5f;

        private Transform targetTransform;
        private Interactable currentTouchable;
        private StopwatchTimer touchCountdownTimer;

        private void Awake()
        {
            touchCountdownTimer = new StopwatchTimer();
        }

        private void Start()
        {
            touchCountdownTimer.Start();
            ResetHandPositions();
        }

        private void LateUpdate()
        {
            if (nearestInteractableFinder == null) return;

            UpdateIKTarget();
            HandleIK();
        }

        private void UpdateIKTarget()
        {
            targetTransform = nearestInteractableFinder?.CurrentTarget?.transform;
            if (targetTransform == null) return;
            currentTouchable = targetTransform.GetComponent<Interactable>();
        }

        private void HandleIK()
        {
            if (targetTransform == null || currentTouchable == null)
            {
                ResetIKWeights();
                ResetHandPositions();
                return;
            }

            if (!IsTargetInRange(targetTransform) || !currentTouchable.IsTouchable)
            {
                ResetIKWeights();
                ResetHandPositions();
                return;
            }

            switch (currentTouchable.HandPoint)
            {
                case HandIKPoint.RightHand:
                    HandleRightHandIK(currentTouchable.RightHandTarget);
                    break;

                case HandIKPoint.LeftHand:
                    HandleLeftHandIK(currentTouchable.LeftHandTarget);
                    break;

                case HandIKPoint.BothHands:
                    HandleClosestHandIK();
                    break;

                case HandIKPoint.TwoHands:
                    HandleTwoHandsIK();
                    break;
            }
        }

        private bool IsTargetInRange(Transform target)
        {
            if (target == null) return false;
            return Vector3.Distance(target.position, chestTransform.position) < handLength;
        }

        private void HandleClosestHandIK()
        {
            // Если есть обе точки — определяем, какая ближе
            float rightDist = currentTouchable.RightHandTarget != null
                ? Vector3.Distance(defaultTransformRight.position, currentTouchable.RightHandTarget.position)
                : float.MaxValue;

            float leftDist = currentTouchable.LeftHandTarget != null
                ? Vector3.Distance(defaultTransformLeft.position, currentTouchable.LeftHandTarget.position)
                : float.MaxValue;

            if (rightDist <= leftDist)
                HandleRightHandIK(currentTouchable.RightHandTarget);
            else
                HandleLeftHandIK(currentTouchable.LeftHandTarget);
        }

        private void HandleRightHandIK(Transform target)
        {
            if (target == null)
            {
                ResetRightHand();
                return;
            }

            rightHandIK.weight = Mathf.Lerp(rightHandIK.weight, 1f, Time.deltaTime * lerpSpeed);
            UpdateHandPositionAndRotation(rightHandTransform, target);

            leftHandIK.weight = Mathf.Lerp(leftHandIK.weight, 0f, Time.deltaTime * lerpSpeed);
            ResetLeftHand();
        }

        private void HandleLeftHandIK(Transform target)
        {
            if (target == null)
            {
                ResetLeftHand();
                return;
            }

            leftHandIK.weight = Mathf.Lerp(leftHandIK.weight, 1f, Time.deltaTime * lerpSpeed);
            UpdateHandPositionAndRotation(leftHandTransform, target);

            rightHandIK.weight = Mathf.Lerp(rightHandIK.weight, 0f, Time.deltaTime * lerpSpeed);
            ResetRightHand();
        }

        private void HandleTwoHandsIK()
        {
            // Обе руки активны
            rightHandIK.weight = Mathf.Lerp(rightHandIK.weight, 1f, Time.deltaTime * lerpSpeed);
            leftHandIK.weight = Mathf.Lerp(leftHandIK.weight, 1f, Time.deltaTime * lerpSpeed);

            if (currentTouchable.RightHandTarget != null)
                UpdateHandPositionAndRotation(rightHandTransform, currentTouchable.RightHandTarget);

            if (currentTouchable.LeftHandTarget != null)
                UpdateHandPositionAndRotation(leftHandTransform, currentTouchable.LeftHandTarget);
        }

        private void UpdateHandPositionAndRotation(Transform handTransform, Transform target)
        {
            if (target == null) return;

            float progress = Mathf.Clamp01(touchCountdownTimer.CurrentTime / touchDuration);
            float curveValue = animationCurve.Evaluate(progress);

            handTransform.position = Vector3.Lerp(handTransform.position, target.position, curveValue);

            Quaternion targetRot = target.rotation;
            if (handTransform == leftHandTransform)
                targetRot *= Quaternion.Euler(0, 180f, 0);

            handTransform.rotation = Quaternion.Lerp(handTransform.rotation, targetRot, curveValue);
        }

        private void ResetHandPositions()
        {
            ResetRightHand();
            ResetLeftHand();
        }

        private void ResetRightHand()
        {
            rightHandTransform.position = Vector3.Lerp(rightHandTransform.position, defaultTransformRight.position, Time.deltaTime * resetLerpSpeed);
            rightHandTransform.rotation = Quaternion.Lerp(rightHandTransform.rotation, defaultTransformRight.rotation, Time.deltaTime * resetLerpSpeed);
        }

        private void ResetLeftHand()
        {
            leftHandTransform.position = Vector3.Lerp(leftHandTransform.position, defaultTransformLeft.position, Time.deltaTime * resetLerpSpeed);
            leftHandTransform.rotation = Quaternion.Lerp(leftHandTransform.rotation, defaultTransformLeft.rotation, Time.deltaTime * resetLerpSpeed);
        }

        private void ResetIKWeights()
        {
            rightHandIK.weight = Mathf.Lerp(rightHandIK.weight, 0f, Time.deltaTime * resetLerpSpeed);
            leftHandIK.weight = Mathf.Lerp(leftHandIK.weight, 0f, Time.deltaTime * resetLerpSpeed);
            touchCountdownTimer.Reset();
        }
    }
}