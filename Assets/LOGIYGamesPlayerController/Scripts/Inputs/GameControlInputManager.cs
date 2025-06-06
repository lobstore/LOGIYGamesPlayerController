using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;
namespace LOGIYGames
{
    public class GameControlInputManager : MonoBehaviour, PlayerInputActions.IGameControlActions
    {
        public static GameControlInputManager Instance { get; private set; }
        public PlayerInputActions InputActions { get; private set; }
        [SerializeField] bool dontDestroyOnLoad;
        public UnityEvent OnExitButtonClicked { get; private set; } = new UnityEvent();
        public UnityEvent<bool> OnVoiceChatStarted { get; private set; } = new();
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
            InputActions.GameControl.Enable();
            InputActions.GameControl.SetCallbacks(this);
        }
        private void OnDisable()
        {
            InputActions.GameControl.Disable();
            InputActions.GameControl.RemoveCallbacks(this);
        }

        public void OnEscape(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    OnExitButtonClicked.Invoke();
                    break;
                default:
                    break;
            }
        }

        public void OnVoice(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    OnVoiceChatStarted.Invoke(true);
                    print("Voice On");
                    break;
                case InputActionPhase.Canceled:
                    OnVoiceChatStarted.Invoke(false);
                    print("Voice Off");
                    break;
                default:
                    break;
            }
        }
    }
}