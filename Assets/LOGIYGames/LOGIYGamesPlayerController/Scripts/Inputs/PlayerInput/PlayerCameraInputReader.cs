using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerCameraInputReader:ICameraInputReader
    {
        InputActionAsset InputActions;
        InputActionMap CameraActionMap;
        InputAction ZoomAction;
        public float ZoomDelta => ZoomAction.ReadValue<float>();

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
            CameraActionMap = InputActions.FindActionMap("Camera");
            ZoomAction = CameraActionMap.FindAction("Zoom");
        }
    }
    public interface ICameraInputReader
    {
        public float ZoomDelta { get; }
    }
}
