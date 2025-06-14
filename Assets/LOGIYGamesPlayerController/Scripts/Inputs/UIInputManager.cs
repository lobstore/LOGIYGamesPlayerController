using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
namespace LOGIYGames
{
    public class UIInputManager : MonoBehaviour, GameInputs.IUIActions
    {
        public static UIInputManager Instance { get; private set; }
        public GameInputs InputActions { get; private set; }
        public UnityEvent Submitted { get; private set; } = new();

        public void OnSubmit(InputAction.CallbackContext context)
        {

            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Submitted.Invoke();
                    break;
                default:
                    break;
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {

                Instance = this;

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void OnEnable()
        {
            InputActions = InputManager.Instance.InputActions;
            InputActions.UI.Enable();
            InputActions.UI.SetCallbacks(this);
        }
        private void OnDisable()
        {
            InputActions.UI.Disable();
            InputActions.UI.RemoveCallbacks(this);
        }
    }
}