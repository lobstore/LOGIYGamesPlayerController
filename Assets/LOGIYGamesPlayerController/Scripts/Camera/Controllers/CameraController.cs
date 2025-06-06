using Cinemachine;
using UnityEngine;
namespace LOGIYGames
{
    public class CameraController : MonoBehaviour
    {

        Camera MainCamera;
        CinemachineVirtualCamera VirtualCamera;
        CinemachineFramingTransposer FramingTransposer;
        public CinemachineInputProvider CameraInputProvider { get; private set; }
        public Transform CameraTransform { get => MainCamera.transform; }

        public Transform CameraFollowTarget { get => VirtualCamera.Follow; set => VirtualCamera.Follow = value; }
        public Transform CameraLookAtTarget { get => VirtualCamera.LookAt; set => VirtualCamera.LookAt = value; }

        public float FOV { get => VirtualCamera.m_Lens.FieldOfView; set => VirtualCamera.m_Lens.FieldOfView = Mathf.Clamp(value, 0, 360); }
        public float Distance { get => FramingTransposer.m_CameraDistance; set => FramingTransposer.m_CameraDistance = Mathf.Clamp(value, 0.1f, 10); }
        public int Priority { get => VirtualCamera.Priority; set => VirtualCamera.Priority = value; }
        private void Awake()
        {
            MainCamera = Camera.main;
            VirtualCamera = GetComponent<CinemachineVirtualCamera>();
            FramingTransposer = VirtualCamera?.GetCinemachineComponent<CinemachineFramingTransposer>();
            CameraInputProvider = VirtualCamera?.GetComponent<CinemachineInputProvider>();
        }
        public Quaternion GetCameraLookRotation()
        {
            Vector3 directionToCamera = MainCamera.transform.forward;
            directionToCamera.y = 0;
            directionToCamera.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            return targetRotation;
        }
    }
}