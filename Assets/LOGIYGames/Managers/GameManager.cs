using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        [SerializeField] InputActionAsset InputActions;

        public GameInputReader GameInput {  get; private set; }
        public bool UIEngaged { get; private set; }
        override protected void Awake()
        {
            base.Awake();
            GameInput = new(InputActions);
            GameInput.Enable();
            GameInput.UIEngageAction.performed += (x) =>
            {
                if (x.performed)
                {
                    UIEngaged = !UIEngaged;
                    if (UIEngaged)
                    {
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        CameraManager.Instance.CameraInput.Enable();
                        PlayerManager.Instance.PlayerInput.Enable();
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }

            };
        }
    }
    public class GameInputReader
    {
        InputActionAsset InputActions;
        InputActionMap GameActionMap;
        public InputAction EscapeAction { get; private set; }
        public InputAction UIEngageAction { get; private set; }
        public bool EscapePressed => EscapeAction.WasPressedThisFrame();
        public void Enable()
        {
            GameActionMap.Enable();
        }
        public void Disable()
        {
            GameActionMap.Disable();
        }
        public GameInputReader(InputActionAsset inputActions)
        {
            InputActions = inputActions;
            GameActionMap = InputActions.FindActionMap("GameControl");
            UIEngageAction = GameActionMap.FindAction("UIEngage");
            EscapeAction = GameActionMap.FindAction("Escape");
        }
    }
}
