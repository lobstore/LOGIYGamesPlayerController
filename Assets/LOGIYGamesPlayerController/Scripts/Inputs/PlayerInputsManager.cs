using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
namespace LOGIYGames
{
    public class PlayerInputsManager : MonoBehaviour, GameInputs.IPlayerInputsActions
    {
        public static PlayerInputsManager Instance { get; private set; }
        [SerializeField] bool dontDestroyOnLoad;
        public GameInputs InputActions { get; private set; }
        public Vector2 MovementInput { get; private set; }

        public bool IsShifting { get; private set; }
        public bool IsCrouching { get; private set; }
        public bool IsHolding { get; private set; }
        public UnityEvent Jumped { get; private set; } = new();
        public UnityEvent Rolled { get; private set; } = new();
        public UnityEvent Interacted { get; private set; } = new();
        public UnityEvent Crouched { get; private set; } = new();
        public UnityEvent Attacked { get; private set; } = new UnityEvent();
        public UnityEvent Blocked { get; private set; } = new UnityEvent();
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
            InputActions.PlayerInputs.Enable();

            InputActions.PlayerInputs.SetCallbacks(this);
        }

        private void OnDisable()
        {
            InputActions.PlayerInputs.Disable();
            InputActions.PlayerInputs.RemoveCallbacks(this);
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

        public void OnSprint(InputAction.CallbackContext context)
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

        public void OnCrouch(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    IsCrouching = true;
                    Crouched.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    IsCrouching = false;
                    break;
                default:
                    break;
            }
        }

        public void OnMap(InputAction.CallbackContext context)
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
        public void OnEvade(InputAction.CallbackContext context)
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
        public void OnAttack(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Attacked.Invoke();
                    break;
                default:
                    break;
            }
        }

        public void OnBlock(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Blocked.Invoke();
                    break;
                default:
                    break;
            }
        }
    }
}