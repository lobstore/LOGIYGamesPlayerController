using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
namespace LOGIYGames
{
    public class MouseInputManager : MonoBehaviour, PlayerInputActions.IMouseActions
    {
        public static MouseInputManager Instance { get; private set; }
        [SerializeField] bool dontDestroyOnLoad;
        public PlayerInputActions InputActions { get; private set; }
        public Vector2 Input { get; private set; }
        public float MiddleScrollingDelta { get; private set; }
        public bool IsTargeting { get; private set; }
        public UnityEvent LCMPressed { get; private set; } = new UnityEvent();
        public UnityEvent RCMPressed { get; private set; } = new UnityEvent();
        public void OnLook(InputAction.CallbackContext context)
        {
            Input = context.ReadValue<Vector2>();
        }

        public void OnMiddleScroll(InputAction.CallbackContext context)
        {
            MiddleScrollingDelta = context.ReadValue<float>();
        }
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
            InputActions.Mouse.Enable();

            InputActions.Mouse.SetCallbacks(this);
        }

        private void OnDisable()
        {
            InputActions.Mouse.Disable();
            InputActions.Mouse.RemoveCallbacks(this);
        }

        public void OnMiddleButton(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    IsTargeting = !IsTargeting;
                    break;
                default:
                    break;
            }
        }

        public void OnLCM(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    LCMPressed.Invoke();
                    break;
                default:
                    break;
            }
        }

        public void OnRCM(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    RCMPressed.Invoke();
                    break;
                default:
                    break;
            }
        }
    }
}