using UnityEngine;

namespace LOGIYGames
{
    [RequireComponent(typeof(Animator))]
    public class HandsIK : MonoBehaviour
    {
        private Animator _animator;
        public Vector3 LeftHandPoint { get; set; }
        public Vector3 LeftHandNormal { get; set; }
        public Vector3 RightHandPoint { get; set; }
        public Vector3 RightHandNormal { get; set; }

        [SerializeField]
        private float ikWeightSpeed = 10f;
        float lHandTarget;
        float rHandTarget;
        private float lHandWeight;
        private float rHandWeight;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void EnableIK()
        {
            EnableRHandIK();
            EnableLHandIK();
        }

        public void DisableIK()
        {
            DisableRHandIK();
            DisableLHandIK();
        }

        public void EnableRHandIK()
        {
            rHandTarget = 1;
        }
        public void EnableLHandIK()
        {
            lHandTarget = 1;
        }
        public void DisableRHandIK()
        {
            rHandTarget = 0;
        }
        public void DisableLHandIK()
        {
            lHandTarget = 0;
        }
        private void Update()
        {

            lHandWeight = Mathf.MoveTowards(
                lHandWeight,
                lHandTarget,
                Time.deltaTime * ikWeightSpeed);
            rHandWeight = Mathf.MoveTowards(
                rHandWeight,
                rHandTarget,
                Time.deltaTime * ikWeightSpeed);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null)
                return;

            ApplyHandIK(AvatarIKGoal.LeftHand, LeftHandPoint, LeftHandNormal, lHandWeight);

            ApplyHandIK(AvatarIKGoal.RightHand, RightHandPoint, RightHandNormal, rHandWeight);
        }

        private void ApplyHandIK(
            AvatarIKGoal goal,
            Vector3 targetPoint,
            Vector3 normal, float weight)
        {
            _animator.SetIKPositionWeight(goal, weight);
            _animator.SetIKRotationWeight(goal, weight);

            _animator.SetIKPosition(goal, targetPoint);

            // Направление вдоль поверхности
            Vector3 alongSurface =
                Vector3.Cross(normal, -transform.forward).normalized;
            //Uncomment if hands not symmetry
            //if (goal == AvatarIKGoal.LeftHand)
            //    alongSurface = -alongSurface;

            // Вверх кисти = нормаль поверхности
            Quaternion surfaceRotation =
                Quaternion.LookRotation(alongSurface, normal);

            // Подстройка под риг
            Quaternion correction =
                Quaternion.Euler(0f, 90f, 0f);

            _animator.SetIKRotation(
                goal,
                surfaceRotation * correction);
        }
    }
}
