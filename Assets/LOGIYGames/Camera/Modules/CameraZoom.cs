using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
namespace LOGIYGames
{
    public class CameraZoom : MonoBehaviour
    {
        PlayerCameraInputReader PlayerCameraInput;

        [SerializeField][Range(0f, 10f)] private float defaultDistance = 4f;
        [SerializeField][Range(0f, 10f)] private float minimumDistance = 1f;
        [SerializeField][Range(0f, 10f)] private float maximumDistance = 6f;

        [SerializeField][Range(0f, 10f)] private float smoothing = 4f;
        [SerializeField][Range(0f, 10f)] private float zoomSensitivity = 1f;
        [SerializeField][Range(0f, 10f)] private float currentTargetDistance;

        private CinemachineFreeLookModifier.IModifiableDistance framingTransposer;

        private float Distance { get { return framingTransposer.Distance; } set { framingTransposer.Distance = value; } }


        private void Awake()
        {
            framingTransposer = GetComponent<CinemachineFreeLookModifier.IModifiableDistance>();

            currentTargetDistance = defaultDistance;
            PlayerCameraInput = CameraManager.Instance.CameraInput;
        }

        private float zoomDelta;

        private void Update()
        {
            zoomDelta = PlayerCameraInput.ZoomDelta;
            Zoom();
        }

        private void Zoom()
        {
            if (zoomDelta!=0)
            {
                currentTargetDistance = Mathf.Clamp(currentTargetDistance + zoomDelta * zoomSensitivity, minimumDistance, maximumDistance);

            }
            Distance = Mathf.Lerp(Distance, currentTargetDistance, smoothing * Time.deltaTime);

        }
    }
}