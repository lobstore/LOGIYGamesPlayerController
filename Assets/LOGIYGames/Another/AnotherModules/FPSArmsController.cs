using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FPSArmsController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera fpsCamera;

    [Header("IK Targets")]
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;

    [Header("Position")]
    [SerializeField] private float handDistance = 0.65f;
    [SerializeField] private float verticalOffset = -0.15f;
    [SerializeField] private float horizontalOffset = 0.25f;

    [Header("Rotation")]
    [SerializeField] private float positionSmooth = 15f;
    [SerializeField] private float rotationSmooth = 15f;

    [Header("IK Weight")]
    [Range(0f, 1f)]
    [SerializeField] private float ikWeight = 1f;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (fpsCamera == null)
            fpsCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (fpsCamera == null)
            return;

        UpdateHandTarget(
            leftHandTarget,
            -horizontalOffset
        );

        UpdateHandTarget(
            rightHandTarget,
            horizontalOffset
        );
    }

    private void UpdateHandTarget(Transform target, float sideOffset)
    {
        if (target == null)
            return;

        Vector3 desiredPosition =
            fpsCamera.transform.position +
            fpsCamera.transform.forward * handDistance +
            fpsCamera.transform.right * sideOffset +
            fpsCamera.transform.up * verticalOffset;

        Quaternion desiredRotation =
            Quaternion.LookRotation(
                fpsCamera.transform.forward,
                fpsCamera.transform.up
            );

        float positionT = 1f - Mathf.Exp(-positionSmooth * Time.deltaTime);
        float rotationT = 1f - Mathf.Exp(-rotationSmooth * Time.deltaTime);

        target.position = Vector3.Lerp(
            target.position,
            desiredPosition,
            positionT
        );

        target.rotation = Quaternion.Slerp(
            target.rotation,
            desiredRotation,
            rotationT
        );
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null)
            return;

        if (leftHandTarget != null)
        {
            animator.SetIKPositionWeight(
                AvatarIKGoal.LeftHand,
                ikWeight
            );

            animator.SetIKRotationWeight(
                AvatarIKGoal.LeftHand,
                ikWeight
            );

            animator.SetIKPosition(
                AvatarIKGoal.LeftHand,
                leftHandTarget.position
            );

            animator.SetIKRotation(
                AvatarIKGoal.LeftHand,
                leftHandTarget.rotation
            );
        }

        if (rightHandTarget != null)
        {
            animator.SetIKPositionWeight(
                AvatarIKGoal.RightHand,
                ikWeight
            );

            animator.SetIKRotationWeight(
                AvatarIKGoal.RightHand,
                ikWeight
            );

            animator.SetIKPosition(
                AvatarIKGoal.RightHand,
                rightHandTarget.position
            );

            animator.SetIKRotation(
                AvatarIKGoal.RightHand,
                rightHandTarget.rotation
            );
        }
    }
}