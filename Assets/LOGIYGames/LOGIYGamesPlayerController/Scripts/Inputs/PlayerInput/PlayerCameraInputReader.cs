using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerCameraInputReader:ICameraInputReader
    {
        InputActionAsset InputActions;
        InputActionMap CameraActionMap;
        public InputAction ZoomAction {  get; private set; }
        public InputAction LookAction {  get; private set; }
        public float ZoomDelta => ZoomAction.ReadValue<float>();
        public Vector2 LookInput => LookAction.ReadValue<Vector2>();

        public void Enable()
        {
            CameraActionMap.Enable();
        }
        public void Disable()
        {
            CameraActionMap.Disable();
        }
        public PlayerCameraInputReader(InputActionAsset inputActions)
        {
            InputActions = inputActions;
            CameraActionMap = InputActions.FindActionMap("CameraInputs");
            ZoomAction = CameraActionMap.FindAction("Zoom");
            LookAction = CameraActionMap.FindAction("Look");
        }
    }
}
