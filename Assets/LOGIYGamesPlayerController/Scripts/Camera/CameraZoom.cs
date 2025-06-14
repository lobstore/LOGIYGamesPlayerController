using Cinemachine;
using UnityEngine;
namespace LOGIYGames
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField][Range(0f, 10f)] private float defaultDistance = 4f;
        [SerializeField][Range(0f, 10f)] private float minimumDistance = 1f;
        [SerializeField][Range(0f, 10f)] private float maximumDistance = 6f;

        [SerializeField][Range(0f, 10f)] private float smoothing = 4f;
        [SerializeField][Range(0f, 10f)] private float zoomSensitivity = 1f;

        private CinemachineFramingTransposer framingTransposer;
        private CinemachineInputProvider inputProvider;

        private float Distance { get { return framingTransposer.m_CameraDistance; } set { framingTransposer.m_CameraDistance = value; } }

        private float currentTargetDistance;

        private void Awake()
        {
            framingTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();

            inputProvider = GetComponent<CinemachineInputProvider>();

            currentTargetDistance = defaultDistance;
        }

        private void Update()
        {
            Zoom();
        }

        private void Zoom()
        {
            float zoomValue = CameraInputManager.Instance.ZoomDelta * zoomSensitivity;
            currentTargetDistance = Mathf.Clamp(currentTargetDistance + zoomValue, minimumDistance, maximumDistance);
            if (Mathf.Abs(Distance - currentTargetDistance) > 0.01f)
            {
                Distance = Mathf.Lerp(Distance, currentTargetDistance, smoothing * Time.deltaTime);

            }
            else { Distance = currentTargetDistance; }
        }
    }
}