using Unity.Cinemachine;
using UnityEngine;

namespace LOGIYGames
{
    public class CameraRotationLimiter : CinemachineExtension
    {
        Transform character;

        [Header("Limit")]
        [Range(0, 180)]
        public float maxYawAngle = 70f;
        protected override void Awake()
        {
            base.Awake();
            PlayerManager.Instance.OnCharacterChanged.AddListener((chr) =>
            {
                character = chr.transform;
            });
        }
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage,
            ref CameraState state,
            float deltaTime)
        {

            if (stage != CinemachineCore.Stage.Aim)
                return;

            if (character == null)
                return;

            Vector3 forward = character.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 cameraForward = state.RawOrientation * Vector3.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            float angle = Vector3.SignedAngle(
                forward,
                cameraForward,
                Vector3.up
            );

            if (Mathf.Abs(angle) > maxYawAngle)
            {
                float clampedAngle = Mathf.Clamp(
                    angle,
                    -maxYawAngle,
                    maxYawAngle
                );

                Quaternion targetRotation =
                    Quaternion.LookRotation(
                        Quaternion.AngleAxis(clampedAngle, Vector3.up) * forward,
                        Vector3.up
                    );

                state.RawOrientation = Quaternion.Euler(
                    targetRotation.eulerAngles.x,
                    targetRotation.eulerAngles.y,
                    state.RawOrientation.eulerAngles.z
                );
            }
        }
    }
}
