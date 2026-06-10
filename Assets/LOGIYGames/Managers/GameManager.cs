using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        [SerializeField] InputActionAsset InputActions;

        InputActionMap GameActionMap;
        InputActionMap CameraActionMap;
        InputActionMap CharacterActionMap;

        InputAction UIEngageAction;

        public bool UIEngaged { get; private set; }
        override protected void Awake()
        {
            base.Awake();
            GameActionMap = InputActions.FindActionMap("GameControl");
            CameraActionMap = InputActions.FindActionMap("Camera");
            CharacterActionMap = InputActions.FindActionMap("CharacterInputs");
            UIEngageAction = GameActionMap.FindAction("UIEngage");
            UIEngageAction.performed += (x) =>
            {
                if (x.performed)
                {
                    UIEngaged = !UIEngaged;
                    if (UIEngaged)
                    {
                        CameraActionMap.Disable();
                        CharacterActionMap.Disable();
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        CameraActionMap.Enable();
                        CharacterActionMap.Enable();
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }

            };
            GameActionMap.Enable();
        }
    }
}
