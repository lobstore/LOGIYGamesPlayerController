using UnityEngine;
using UnityEngine.Animations.Rigging;
namespace LOGIYGames
{
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

        private Transform targetTransform;
        private Interactable currentTouchable;
        private StopwatchTimer touchCountdownTimer;
        private float lerpSpeed = 10f;
        private float resetLerpSpeed = 5f;

        private void Awake()
        {
            touchCountdownTimer = new StopwatchTimer();
        }
        private void Start()
        {

            touchCountdownTimer.Start();
            ResetHandPositions();
        }

        private void Update()
        {
            touchCountdownTimer.Tick(Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (nearestInteractableFinder == null) return;
            UpdateIKTarget();
            HandleIKWeights();
        }

        private void ResetHandPositions()
        {
            rightHandTransform.position = Vector3.Lerp(rightHandTransform.position, defaultTransformRight.position, Time.deltaTime * resetLerpSpeed);
            leftHandTransform.position = Vector3.Lerp(leftHandTransform.position, defaultTransformLeft.position, Time.deltaTime * resetLerpSpeed);

            // Reset hand rotations to default
            rightHandTransform.rotation = Quaternion.Lerp(rightHandTransform.rotation, defaultTransformRight.rotation, Time.deltaTime * resetLerpSpeed);
            leftHandTransform.rotation = Quaternion.Lerp(leftHandTransform.rotation, defaultTransformLeft.rotation, Time.deltaTime * resetLerpSpeed);
        }

        private void UpdateIKTarget()
        {

            targetTransform = nearestInteractableFinder?.CurrentTarget?.transform;
            if (targetTransform == null) return;
            currentTouchable = targetTransform.GetComponent<Interactable>();
        }

        private void HandleIKWeights()
        {
            if (targetTransform == null)
            {
                ResetIKWeights();
                ResetHandPositions();
                return;
            }

            if (!IsTargetInRange(currentTouchable?.Origin) || !currentTouchable.IsTouchable)
            {
                ResetIKWeights();
                ResetHandPositions();
                return;
            }

            if (IsRightHandCloser())
            {
                HandleRightHandIK();
            }
            else
            {
                HandleLeftHandIK();
            }
        }

        private bool IsTargetInRange(Transform targettransform)
        {
            if (targettransform != null)
            {
                return Vector3.Distance(targettransform.position, chestTransform.position) < handLength;

            }
            else
            {
                return default;
            }
        }

        private bool IsRightHandCloser()
        {
            return Vector3.Distance(defaultTransformRight.position, currentTouchable.Origin.position) <
                   Vector3.Distance(defaultTransformLeft.position, currentTouchable.Origin.position);
        }

        private void HandleRightHandIK()
        {
            rightHandIK.weight = Mathf.Lerp(rightHandIK.weight, 1f, Time.deltaTime * lerpSpeed);
            UpdateHandPositionAndRotation(rightHandTransform, targetTransform.position, currentTouchable.Origin);

            leftHandIK.weight = Mathf.Lerp(leftHandIK.weight, 0f, Time.deltaTime * lerpSpeed);
            leftHandTransform.position = Vector3.Lerp(leftHandTransform.position, defaultTransformLeft.position, Time.deltaTime * resetLerpSpeed);
            leftHandTransform.rotation = Quaternion.Lerp(leftHandTransform.rotation, defaultTransformLeft.rotation, Time.deltaTime * resetLerpSpeed);
        }

        private void HandleLeftHandIK()
        {
            leftHandIK.weight = Mathf.Lerp(leftHandIK.weight, 1f, Time.deltaTime * lerpSpeed);
            UpdateHandPositionAndRotation(leftHandTransform, targetTransform.position, currentTouchable.Origin);

            rightHandIK.weight = Mathf.Lerp(rightHandIK.weight, 0f, Time.deltaTime * lerpSpeed);
            rightHandTransform.position = Vector3.Lerp(rightHandTransform.position, defaultTransformRight.position, Time.deltaTime * resetLerpSpeed);
            rightHandTransform.rotation = Quaternion.Lerp(rightHandTransform.rotation, defaultTransformRight.rotation, Time.deltaTime * resetLerpSpeed);
        }

        private void UpdateHandPositionAndRotation(Transform handTransform, Vector3 targetPosition, Transform targetOrigin)
        {
            float touchProgress = Mathf.Clamp01(touchCountdownTimer.GetTime() / touchDuration);
            float curveValue = animationCurve.Evaluate(touchProgress);

            if (targetOrigin != null)
            {
                handTransform.position = Vector3.Lerp(handTransform.position, targetOrigin.position, curveValue);

                Quaternion targetRotation;
                if (handTransform == rightHandTransform)
                {
                    targetRotation = targetOrigin.rotation;
                }
                else
                {
                    targetRotation = targetOrigin.rotation * Quaternion.Euler(0, 180f, 0);
                }

                handTransform.rotation = Quaternion.Lerp(handTransform.rotation, targetRotation, curveValue);
            }
        }

        private void ResetIKWeights()
        {
            rightHandIK.weight = Mathf.Lerp(rightHandIK.weight, 0f, Time.deltaTime * resetLerpSpeed);
            leftHandIK.weight = Mathf.Lerp(leftHandIK.weight, 0f, Time.deltaTime * resetLerpSpeed);
            touchCountdownTimer.Reset();
        }
    }
}