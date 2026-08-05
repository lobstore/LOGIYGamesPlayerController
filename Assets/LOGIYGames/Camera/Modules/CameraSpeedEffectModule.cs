using LOGIYGames.Movement;
using Unity.Cinemachine;
using UnityEngine;

namespace LOGIYGames
{
    public class CameraSpeedEffectModule : MonoBehaviour
    {
        private CinemachineCamera cinemachineCamera;

        [Header("FOV")]
        [SerializeField] private float normalFov = 75f;
        [SerializeField] private float sprintFov = 90f;
        [SerializeField] private float smooth = 8f;

        private SprintMovementState sprintMovementState;

        private void Start()
        {
            cinemachineCamera = GetComponent<CinemachineCamera>();
            PlayerManager.Instance.OnCharacterChanged.AddListener(chr =>
            {
                sprintMovementState = chr.GetMovementState<SprintMovementState>();

                // Запоминаем текущий FOV, если он задан в камере
                normalFov = cinemachineCamera.Lens.FieldOfView;
            });
        }

        private void Update()
        {
            if (sprintMovementState == null)
                return;

            float targetFov = sprintMovementState.IsActiveState
                ? sprintFov
                : normalFov;

            var lens = cinemachineCamera.Lens;
            lens.FieldOfView = Mathf.Lerp(
                lens.FieldOfView,
                targetFov,
                smooth * Time.deltaTime);

            cinemachineCamera.Lens = lens;
        }
    }
}
