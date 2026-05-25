using UnityEngine;
namespace LOGIYGames.CharacterCore
{
    public class CameraTargetModule : MonoModuleBase
    {
        [Header("Camera Configuration")]
        [SerializeField] Transform cameraLookAt;
        [SerializeField] Transform cameraFollow;
        public Transform CameraLookAt => cameraLookAt;
        public Transform CameraFollow => cameraFollow;

    }
}
