using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames
{
    public class CameraInputManager : MonoBehaviour, GameInputs.ICameraActions
    {
        public static CameraInputManager Instance { get; private set; }
        [SerializeField] bool dontDestroyOnLoad;
        public GameInputs InputActions { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsFocusing {  get; private set; }
        public float ZoomDelta { get; private set; }

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
            InputActions.Camera.Enable();

            InputActions.Camera.SetCallbacks(this);
        }

        private void OnDisable()
        {
            InputActions.Camera.Disable();
            InputActions.Camera.RemoveCallbacks(this);
        }

        public void OnLook(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }

        public void OnZoom(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            context.ReadValue<float>();
        }

        public void OnFocus(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }
    }
}
