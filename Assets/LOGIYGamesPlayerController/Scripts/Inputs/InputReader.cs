using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputEvent : UnityEvent<InputAction.CallbackContext> { }

namespace LOGIYGames
{
    [CreateAssetMenu(menuName ="Input/InputReader", fileName ="InputReader")]
    public class InputReader : ScriptableObject, GameInputs.IPlayerInputsActions, GameInputs.IGameControlActions, GameInputs.IUIActions, GameInputs.ICameraActions
    {

        private GameInputs gameInputs;

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
        public InputEvent MapEvent { get; private set; } = new();
        public InputEvent SprintEvent { get; private set; } = new();
        public InputEvent VoiceChatEvent { get; private set; } = new();


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
            EnableInputs();
        }

        private void OnDisable()
        {
            if(gameInputs != null)
            {
                gameInputs.UI.RemoveCallbacks(this);
                gameInputs.PlayerInputs.RemoveCallbacks(this);
                gameInputs.GameControl.RemoveCallbacks(this);
                gameInputs.Camera.RemoveCallbacks(this);
            }
            DisableInputs();
        }
        public void EnableInputs()
        {
            Debug.Log("EnebleInputReader");
            gameInputs.Enable();
        }
        public void DisableInputs()
        {
            Debug.Log("DisableInputReader");
            gameInputs.Disable();
        }
        public void OnAttack(InputAction.CallbackContext context)
        {
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
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnMap(InputAction.CallbackContext context)
        {
            MapEvent.Invoke(context);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
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
