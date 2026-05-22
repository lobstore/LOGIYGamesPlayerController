using LOGIYGames.CharacterCore;
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
        float target;
        private float _ikWeight;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void EnableIK()
        {
            target = 1;
        }

        public void DisableIK()
        {
            target = 0;
        }

        private void Update()
        {

            _ikWeight = Mathf.MoveTowards(
                _ikWeight,
                target,
                Time.deltaTime * ikWeightSpeed);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null)
                return;

            ApplyHandIK(
                AvatarIKGoal.LeftHand,
                LeftHandPoint,
                LeftHandNormal);

            ApplyHandIK(
                AvatarIKGoal.RightHand,
                RightHandPoint,
                RightHandNormal);
        }

        private void ApplyHandIK(
            AvatarIKGoal goal,
            Vector3 targetPoint,
            Vector3 normal)
        {
            _animator.SetIKPositionWeight(goal, _ikWeight);
            _animator.SetIKRotationWeight(goal, _ikWeight);

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
