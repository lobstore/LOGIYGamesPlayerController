using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
namespace LOGIYGames
{
    public class PlayerMovementInputManager : MonoBehaviour, PlayerInputActions.IPlayerMovementActions
    {
        public static PlayerMovementInputManager Instance { get; private set; }
        [SerializeField] bool dontDestroyOnLoad;
        public PlayerInputActions InputActions { get; private set; }
        public Vector2 MovementInput { get; private set; }

        public bool IsShifting { get; private set; }
        public bool IsCrouching { get; private set; }
        public bool IsHolding { get; private set; }
        public UnityEvent Jumped { get; private set; } = new();
        public UnityEvent Rolled { get; private set; } = new();
        public UnityEvent Interacted { get; private set; } = new();
        public UnityEvent CtrlPressed { get; private set; } = new();
        public bool IsRollButtonPressed { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void OnEnable()
        {
            InputActions = InputManager.Instance.InputActions;
            InputActions.PlayerMovement.Enable();

            InputActions.PlayerMovement.SetCallbacks(this);
        }

        private void OnDisable()
        {
            InputActions.PlayerMovement.Disable();
            InputActions.PlayerMovement.RemoveCallbacks(this);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Jumped.Invoke();
                    break;
                default:
                    break;
            }

        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Interacted.Invoke();
                    break;
                default:
                    break;
            }
        }

        public void OnShift(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    IsShifting = true;
                    break;
                case InputActionPhase.Canceled:
                    IsShifting = false;
                    break;
                default:
                    break;
            }
        }

        public void OnCtrl(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    IsCrouching = true;
                    CtrlPressed.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    IsCrouching = false;
                    break;
                default:
                    break;
            }
        }

        public void OnH(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    IsHolding = true;
                    break;
                case InputActionPhase.Canceled:
                    IsHolding = false;
                    break;
                default:
                    break;
            }
        }

        public void OnQ(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Rolled.Invoke();
                    IsRollButtonPressed = true;
                    break;
                case InputActionPhase.Canceled:
                    IsRollButtonPressed = false;
                    break;
                default:
                    break;
            }
        }
    }
}