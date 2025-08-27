using Unity.Cinemachine;
using UnityEngine;
namespace LOGIYGames
{
    public class CinemachineCameraController : MonoBehaviour
    {

        Camera MainCamera;
        CinemachineCamera VirtualCamera;
        CinemachinePositionComposer CameraComposer;
        public Transform CameraTransform { get => MainCamera.transform; }
        public Transform CameraFollowTarget { get => VirtualCamera.Follow; set => VirtualCamera.Follow = value; }
        public Transform CameraLookAtTarget { get => VirtualCamera.LookAt; set => VirtualCamera.LookAt = value; }

        public float FOV { get => VirtualCamera.Lens.FieldOfView; set => VirtualCamera.Lens.FieldOfView = Mathf.Clamp(value, 0, 360); }
        public float Distance { get => CameraComposer.CameraDistance; set => CameraComposer.CameraDistance = Mathf.Clamp(value, 0.1f, 10); }
        public int Priority { get => VirtualCamera.Priority; set => VirtualCamera.Priority = value; }
        private void Awake()
        {
            MainCamera = Camera.main;
            VirtualCamera = GetComponent<CinemachineCamera>();
            CameraComposer = GetComponent<CinemachinePositionComposer>();
        }

    }
}