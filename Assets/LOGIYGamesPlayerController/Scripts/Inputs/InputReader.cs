using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


namespace LOGIYGames
{
    public class InputEvent : UnityEvent<InputAction.CallbackContext> { }
    [CreateAssetMenu(menuName = "Input/InputReader", fileName = "InputReader")]
    public class InputReader : ScriptableObject, GameInputs.IPlayerInputsActions, GameInputs.IGameControlActions, GameInputs.IUIActions, GameInputs.ICameraActions
    {
        private GameInputs gameInputs;
        public GameInputs GameInputs => gameInputs;
        GameObject lastUISelection;
        GameObject firstUISelection;

        public bool PlayerInputsEnable
        {
           
            get => gameInputs.PlayerInputs.enabled;
            set
            {
                if (value)
                {
                    gameInputs.PlayerInputs.Enable();
                    Debug.Log("EnabledPlayerInputs");
                }
                else
                {
                    gameInputs.PlayerInputs.Disable();
                    Debug.Log("DisabledPlayerInputs");
                }
            }
        }

        public bool CameraInputsEnable
        {
            get => gameInputs.Camera.enabled;
            set
            {
                if (value)
                {
                    gameInputs.Camera.Enable();
                    Debug.Log("EnableCameraInputs");
                }
                else
                {
                    gameInputs.Camera.Disable();
                    Debug.Log("DisableCameraInputs");
                }
            }
        }
        public bool UIInputsEnable
        {
            get => gameInputs.UI.enabled;
            set
            {
                if (value)
                {
                    EnableUIInputs();
                }
                else
                {
                    DisableUIInputs();
                }
            }
        }
        public Vector2 LookInput { get; private set; }
        public Vector2 MoveInput { get; private set; }
        public float ZoomDelta { get; private set; }
        public InputEvent JumpEvent { get; private set; } = new();
        public InputEvent EvadeEvent { get; private set; } = new();
        public InputEvent InteractEvent { get; private set; } = new();
        public InputEvent CrouchEvent { get; private set; } = new();
        public InputEvent AttackEvent { get; private set; } = new();
        public InputEvent BlockEvent { get; private set; } = new();
        public InputEvent FocusingEvent { get; private set; } = new();
        public InputEvent ExitEvent { get; private set; } = new();
        public InputEvent ShowMapEvent { get; private set; } = new();
        public InputEvent SprintEvent { get; private set; } = new();
        public InputEvent VoiceChatEvent { get; private set; } = new();
        public InputEvent MoveEvent { get; private set; } = new();
        public UnityEvent UIEngaged { get; private set; } = new();
        public UnityEvent UIDisengaged { get; private set; } = new();

        private void OnEnable()
        {

            if (gameInputs == null)
            {
                gameInputs = new GameInputs();
                gameInputs.UI.SetCallbacks(this);
                gameInputs.PlayerInputs.SetCallbacks(this);
                gameInputs.GameControl.SetCallbacks(this);
                gameInputs.Camera.SetCallbacks(this);
            }

        }

        private void OnDisable()
        {
            if (gameInputs != null)
            {
                gameInputs.UI.RemoveCallbacks(this);
                gameInputs.PlayerInputs.RemoveCallbacks(this);
                gameInputs.GameControl.RemoveCallbacks(this);
                gameInputs.Camera.RemoveCallbacks(this);
            }
        }
        public void EnableAllInputs()
        {
            Debug.Log("EnableInputReader");
            gameInputs.Enable();
            //var evetnSystem = EventSystem.current;
            //if (evetnSystem == null)
            //{
            //    Debug.Log("None EventSystem was found in scene");
            //    return;
            //}
            //var uiModule = evetnSystem.GetComponent<InputSystemUIInputModule>();
            //if (uiModule == null)
            //{
            //    Debug.Log("None InputSystemUIInputModule was found in scene");
            //    return;
            //}
            //if (uiModule.actionsAsset != gameInputs.asset)
            //{
            //    uiModule.actionsAsset = gameInputs.asset;
            //    Debug.Log("Successfully assigned gameInputs.asset to InputSystemUIInputModule");
            //}
            //Debug.Log("Initialized InputReader");
        }
        public void DisableAllInputs()
        {
            Debug.Log("DisableInputReader");
            gameInputs.Disable();
        }

        private void EnableUIInputs()
        {
            Debug.Log("EnableUIInputs");
            gameInputs.UI.Enable();
            var evetnSystem = EventSystem.current;
            if (evetnSystem == null)
            {
                Debug.Log("None EventSystem was found in scene");
                return;
            }
            var uiModule = evetnSystem.GetComponent<InputSystemUIInputModule>();
            uiModule.actionsAsset.Enable();
        }
        private void DisableUIInputs()
        {
            Debug.Log("DisableUIInputs");
            gameInputs.UI.Enable();
            var evetnSystem = EventSystem.current;
            if (evetnSystem == null)
            {
                Debug.Log("None EventSystem was found in scene");
                return;
            }
            var uiModule = evetnSystem.GetComponent<InputSystemUIInputModule>();
            uiModule.actionsAsset.Disable();
        }
        public bool IsMouseDevice { get; private set; }
        bool IsMouse(InputAction.CallbackContext context) => IsMouseDevice = context.control.device is Mouse;

        public bool IsUIEngaged { get; private set; }
        public void OnUIEngage(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            IsUIEngaged = !IsUIEngaged;

            if (IsUIEngaged)
            {
                if (lastUISelection == null) lastUISelection = firstUISelection;
                EventSystem.current.SetSelectedGameObject(lastUISelection);
                PlayerInputsEnable = false;

                Cursor.lockState = CursorLockMode.None;
                UIEngaged?.Invoke();
            }
            else
            {
                lastUISelection = EventSystem.current.currentSelectedGameObject;
                EventSystem.current.SetSelectedGameObject(null);
                PlayerInputsEnable = true;
                Cursor.lockState = CursorLockMode.Locked;
                UIDisengaged?.Invoke();
            }
        }


        readonly PointerEventData pointerEventData = new(EventSystem.current);
        readonly List<RaycastResult> raycastResults = new();
        bool IsPointerOverUI(Vector2 screenPosition)
        {
            pointerEventData.position = screenPosition;
            raycastResults.Clear();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            return raycastResults.Count > 0;
        }
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            var pointer = Pointer.current;
            if (pointer != null && IsPointerOverUI(pointer.position.ReadValue())) { Debug.Log("Hover UI"); return; }
            AttackEvent.Invoke(context);
        }

        public void OnBlock(InputAction.CallbackContext context)
        {
            BlockEvent.Invoke(context);
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            CrouchEvent.Invoke(context);
        }

        public void OnEscape(InputAction.CallbackContext context)
        {
            ExitEvent.Invoke(context);
        }

        public void OnEvade(InputAction.CallbackContext context)
        {
            EvadeEvent.Invoke(context);
        }

        public void OnFocus(InputAction.CallbackContext context)
        {
            FocusingEvent.Invoke(context);
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            InteractEvent.Invoke(context);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            JumpEvent.Invoke(context);
            IsMouse(context);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnMap(InputAction.CallbackContext context)
        {
            ShowMapEvent.Invoke(context);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent.Invoke(context);
            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            SprintEvent.Invoke(context);
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {

        }

        public void OnVoice(InputAction.CallbackContext context)
        {
            VoiceChatEvent.Invoke(context);
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            ZoomDelta = context.ReadValue<float>();
        }

    }
}
