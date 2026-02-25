using Unity.Cinemachine;
using UnityEngine;
namespace LOGIYGames
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] InputReader Input;
        [SerializeField][Range(0f, 10f)] private float defaultDistance = 4f;
        [SerializeField][Range(0f, 10f)] private float minimumDistance = 1f;
        [SerializeField][Range(0f, 10f)] private float maximumDistance = 6f;

        [SerializeField][Range(0f, 10f)] private float smoothing = 4f;
        [SerializeField][Range(0f, 10f)] private float zoomSensitivity = 1f;
        [SerializeField][Range(0f, 10f)] private float currentTargetDistance;

        private CinemachinePositionComposer framingTransposer;
        private CinemachineInputAxisController inputProvider;

        private float Distance { get { return framingTransposer.CameraDistance; } set { framingTransposer.CameraDistance = value; } }


        private void Awake()
        {
            framingTransposer = GetComponent<CinemachinePositionComposer>();

            inputProvider = GetComponent<CinemachineInputAxisController>();

            currentTargetDistance = defaultDistance;
        }

        private void Update()
        {
            print(Input.ZoomDelta);
            print(Input.CameraInputsEnabled);
            Zoom();
        }

        private void Zoom()
        {
            float zoomDelta = Input.ZoomDelta * zoomSensitivity;

            currentTargetDistance = Mathf.Clamp(currentTargetDistance + zoomDelta, minimumDistance, maximumDistance);
            if (Mathf.Abs(Distance - currentTargetDistance) > 0.01f)
            {
                Distance = Mathf.Lerp(Distance, currentTargetDistance, smoothing * Time.deltaTime);

            }
            else { Distance = currentTargetDistance; }
        }
    }
}