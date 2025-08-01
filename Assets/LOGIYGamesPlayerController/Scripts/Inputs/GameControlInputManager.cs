using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;
namespace LOGIYGames
{
    public class GameControlInputManager : MonoBehaviour, GameInputs.IGameControlActions
    {
        public static GameControlInputManager Instance { get; private set; }
        public GameInputs InputActions { get; private set; }
        [SerializeField] bool dontDestroyOnLoad;
        public UnityEvent Exited { get; private set; } = new UnityEvent();
        public UnityEvent<bool> VoiceChatPressed { get; private set; } = new();
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
                    Exited.Invoke();
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
                    VoiceChatPressed.Invoke(true);
                    print("Voice On");
                    break;
                case InputActionPhase.Canceled:
                    VoiceChatPressed.Invoke(false);
                    print("Voice Off");
                    break;
                default:
                    break;
            }
        }

        public void OnUIEngage(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}