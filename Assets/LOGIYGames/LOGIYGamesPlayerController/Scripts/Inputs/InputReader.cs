using LOGIYGames.CharacterCore;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


namespace LOGIYGames
{
    public class InputEvent : UnityEvent<InputAction.CallbackContext> { }
    [CreateAssetMenu(menuName = "Input/InputReader", fileName = "InputReader")]
    public class InputReader : ScriptableObject,IMovementInputReader, GameInputs.ICharacterInputsActions, GameInputs.IGameControlActions, GameInputs.IUIActions, GameInputs.ICameraActions
    {
        private GameInputs gameInputs;
        public GameInputs GameInputs => gameInputs;
        GameObject lastUISelection;
        GameObject firstUISelection;

        public bool CharacterInputsEnabled
        {
           
            get => gameInputs.CharacterInputs.enabled;
            set
            {
                if (value)
                {
                    gameInputs.CharacterInputs.Enable();
                    Debug.Log("EnabledCharacterInputs");
                }
                else
                {
                    gameInputs.CharacterInputs.Disable();
                    Debug.Log("DisabledCharacterInputs");
                }
            }
        }

        public bool CameraInputsEnabled
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
        public bool UIInputsEnabled
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
        public bool GameControlInputsEnabled
        {
            get => gameInputs.GameControl.enabled;
            set
            {
                if (value)
                {
                    gameInputs.GameControl.Enable();
                }
                else
                {
                    gameInputs.GameControl.Disable();
                }
            }
        }


        public Vector2 LookInput { get; private set; }
        public Vector2 MovementInput { get; private set; }
        public float ZoomDelta { get; private set; }
        public bool JumpPressed { get; set; }
        public bool CrouchPressed { get; private set; }
        public bool AttackPressed { get; private set; }
        public bool SprintPressed { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool EvadePressed { get; private set; }
        public bool BlockPressed { get; private set; }
        public bool FocusPressed { get; private set; }

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
        public event UnityAction<RaycastHit> Click = delegate { };
        private void OnEnable()
        {

            if (gameInputs == null)
            {
                gameInputs = new GameInputs();
                gameInputs.UI.SetCallbacks(this);
                gameInputs.CharacterInputs.SetCallbacks(this);
                gameInputs.GameControl.SetCallbacks(this);
                gameInputs.Camera.SetCallbacks(this);
            }

        }

        private void OnDisable()
        {
            if (gameInputs != null)
            {
                gameInputs.UI.RemoveCallbacks(this);
                gameInputs.CharacterInputs.RemoveCallbacks(this);
                gameInputs.GameControl.RemoveCallbacks(this);
                gameInputs.Camera.RemoveCallbacks(this);
            }
        }
        public void EnableAllInputs()
        {
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

        public bool IsUIEngaged { get; private set; }
        public void EngageUI()
        {
            if (lastUISelection == null) lastUISelection = firstUISelection;
            EventSystem.current.SetSelectedGameObject(lastUISelection);
            CharacterInputsEnabled = false;
            CameraInputsEnabled = false;
            Cursor.lockState = CursorLockMode.None;
            IsUIEngaged = true;
            UIEngaged?.Invoke();


        }
        public void DisengageUI()
        {
            lastUISelection = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);
            CharacterInputsEnabled = true;
            CameraInputsEnabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            IsUIEngaged = false;
            UIDisengaged?.Invoke();
        }
        public void OnUIEngage(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            IsUIEngaged = !IsUIEngaged;
            if (IsUIEngaged)
            {
                EngageUI();
            }
            else
            {
                DisengageUI();
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
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    if (context.phase == InputActionPhase.Started)
                    {
                        if (IsDeviceMouse(context))
                        {
                            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                            if (Physics.Raycast(ray.origin, ray.direction, out var hit, 100))
                            {
                                Click.Invoke(hit);
                            }
                        }
                    }
                    break;
                case InputActionPhase.Performed:
                    AttackPressed = true;
                    break;
                case InputActionPhase.Canceled:
                    AttackPressed = false;
                    break;
                default:
                    break;
            }
            var pointer = Pointer.current;
            //if (pointer != null && IsPointerOverUI(pointer.position.ReadValue())) { Debug.Log("Hover UI"); return; }
            AttackEvent.Invoke(context);
        }
        bool IsDeviceMouse(InputAction.CallbackContext context)
        {
            // Debug.Log($"Device name: {context.control.device.name}");
            return context.control.device.name == "Mouse";
        }
        public void OnBlock(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    BlockPressed = true;
                    break;
                case InputActionPhase.Canceled:
                    BlockPressed = false;
                    break;
                default:
                    break;
            }
            BlockEvent.Invoke(context);
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    CrouchPressed = true;
                    break;
                case InputActionPhase.Canceled:
                    CrouchPressed = false;
                    break;
                default:
                    break;
            }
            CrouchEvent.Invoke(context);
        }

        public void OnEscape(InputAction.CallbackContext context)
        {
            ExitEvent.Invoke(context);
        }

        public void OnEvade(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    EvadePressed = true;
                    break;
                case InputActionPhase.Canceled:
                    EvadePressed = false;
                    break;
                default:
                    break;
            }
            EvadeEvent.Invoke(context);
        }

        public void OnFocus(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    FocusPressed = !FocusPressed;
                    break;
                default:
                    break;
            }
  
            FocusingEvent.Invoke(context);
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    InteractPressed = true;
                    break;
                case InputActionPhase.Canceled:
                    InteractPressed = false;
                    break;
                default:
                    break;
            }
            InteractEvent.Invoke(context);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    JumpPressed = true;
                    break;
                case InputActionPhase.Canceled:
                    JumpPressed = false;
                    break;
                default:
                    break;
            }
            JumpEvent.Invoke(context);
            IsDeviceMouse(context);
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
            MovementInput = context.ReadValue<Vector2>();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    SprintPressed = true;
                    break;
                case InputActionPhase.Canceled:
                    SprintPressed = false;
                    break;
                default:
                    break;
            }
 
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
