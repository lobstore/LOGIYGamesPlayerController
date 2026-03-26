using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerCameraInputReader:MonoBehaviour
    {
        [SerializeField] InputActionAsset InputActions;
        InputActionMap CameraActionMap;
        InputAction ZoomAction;
        public float ZoomDelta {  get; private set; }

        private void Update()
        {
            ZoomDelta = ZoomAction.ReadValue<float>();
        }

        public void Enable()
        {
            CameraActionMap.Enable();
        }
        public void Disable()
        {
            CameraActionMap.Disable();
        }
        private void Awake()
        {
            CameraActionMap = InputActions.FindActionMap("Camera");
            ZoomAction = CameraActionMap.FindAction("Zoom");
        }
    }
}
