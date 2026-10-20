using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace LOGIYGames
{
    public class HandsIK : MonoBehaviour
    {
        [SerializeField] private TwoBoneIKConstraint leftConstraint;
        [SerializeField] private TwoBoneIKConstraint rightConstraint;

        private Transform leftTarget;
        private Transform rightTarget;

        [SerializeField] private float weightSpeed = 10f;

        [SerializeField] private Vector3 leftRotationOffset;
        [SerializeField] private Vector3 rightRotationOffset;

        public Vector3 LeftHandPoint { get; set; }
        public Vector3 LeftHandNormal { get; set; }

        public Vector3 RightHandPoint { get; set; }
        public Vector3 RightHandNormal { get; set; }

        private float _leftWeight;
        private float _rightWeight;

        private float _leftTargetWeight;
        private float _rightTargetWeight;
        private void Awake()
        {
            leftTarget = leftConstraint.data.target;
            rightTarget = rightConstraint.data.target;

        }
        private void Update()
        {
            var delta = Time.deltaTime * weightSpeed;
            _leftWeight = Mathf.MoveTowards(
                _leftWeight,
                _leftTargetWeight,
                delta * (_leftTargetWeight > 0f ? 1f : 2f));
            _rightWeight = Mathf.MoveTowards(
                _rightWeight,
                _rightTargetWeight,
                delta * (_rightTargetWeight > 0f ? 1f : 2f));



            leftConstraint.weight = _leftWeight;
            rightConstraint.weight = _rightWeight;

            SetTarget(
                leftTarget,
                LeftHandPoint,
                LeftHandNormal,
                leftRotationOffset);

            SetTarget(
                rightTarget,
                RightHandPoint,
                RightHandNormal,
                rightRotationOffset);
        }

        public void EnableIK()
        {
            EnableLHandIK();
            EnableRHandIK();
        }

        public void DisableIK()
        {
            DisableLHandIK();
            DisableRHandIK();
        }

        public void EnableLHandIK() => _leftTargetWeight = 1f;
        public void DisableLHandIK() => _leftTargetWeight = 0f;

        public void EnableRHandIK() => _rightTargetWeight = 1f;
        public void DisableRHandIK() => _rightTargetWeight = 0f;

        private void SetTarget(Transform target, Vector3 point, Vector3 normal, Vector3 rotationOffset)
        {
            if (target == null)
                return;

            normal.Normalize();

            Vector3 forward =
                Vector3.Cross(normal, -transform.forward);

            forward.Normalize();

            target.SetPositionAndRotation(
                point,
                Quaternion.LookRotation(forward, normal) *
                Quaternion.Euler(rotationOffset));
        }
    }
}