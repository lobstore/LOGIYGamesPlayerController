using Unity.Cinemachine;
using UnityEngine;
namespace LOGIYGames
{
    public class CinemachineCameraController : MonoBehaviour
    {

        CinemachineCamera VirtualCamera;
        CinemachinePositionComposer CameraComposer;
        CinemachineFollowZoom FollowZoom;
        public Transform CameraFollowTarget { get => VirtualCamera?.Follow; set { if (VirtualCamera != null) { VirtualCamera.Follow = value; } } }
        public Transform CameraLookAtTarget { get => VirtualCamera?.LookAt; set { if (VirtualCamera != null) VirtualCamera.LookAt = value; } }

        public Vector2 FOVRange { get => FollowZoom.FovRange; set => FollowZoom.FovRange = value; }
        public float FOV { get => VirtualCamera.Lens.FieldOfView; set => VirtualCamera.Lens.FieldOfView = value; }
        public float Distance { get => CameraComposer.CameraDistance; set => CameraComposer.CameraDistance = Mathf.Clamp(value, 0.1f, 10); }
        public int Priority { get => VirtualCamera.Priority; set { if (VirtualCamera != null) VirtualCamera.Priority = value; } }
        private void Awake()
        {
            VirtualCamera = GetComponent<CinemachineCamera>();
            CameraComposer = GetComponent<CinemachinePositionComposer>();
            FollowZoom = GetComponent<CinemachineFollowZoom>();
        }

    }
}