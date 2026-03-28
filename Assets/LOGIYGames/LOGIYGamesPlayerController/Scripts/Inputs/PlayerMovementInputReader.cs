using LOGIYGames.CharacterCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerMovementInputReader :MonoBehaviour, IMovementInputReader
    {
        [SerializeField] InputActionAsset InputActions;

        InputActionMap CharacterActionMap;

        InputAction m_MoveAction;
        InputAction m_JumpAction;
        InputAction m_EvadeAction;
        InputAction m_CrouchAction;
        InputAction m_SprintAction;
        InputAction m_FocusAction;
        InputAction m_AttackAction;

        public void Enable()
        {
            CharacterActionMap.Enable();
        }
        public void Disable()
        {
            CharacterActionMap.Disable();
        }
        private void Awake()
        {
            CharacterActionMap = InputActions.FindActionMap("CharacterInputs");
            m_MoveAction = CharacterActionMap.FindAction("Move");
            m_JumpAction = CharacterActionMap.FindAction("Jump");
            m_EvadeAction = CharacterActionMap.FindAction("Evade");
            m_CrouchAction = CharacterActionMap.FindAction("Crouch");
            m_SprintAction = CharacterActionMap.FindAction("Sprint");
            m_FocusAction = CharacterActionMap.FindAction("Focus");
            m_AttackAction = CharacterActionMap.FindAction("Attack");
        }
        private void Update()
        {
            MovementInput = m_MoveAction.ReadValue<Vector2>();
            JumpPressed = m_JumpAction.WasPressedThisFrame();
            EvadePressed = m_EvadeAction.WasPressedThisFrame();
            SprintPressing = m_SprintAction.IsPressed();
            CrouchPressed = m_CrouchAction.IsPressed();
            FocusPressed = m_FocusAction.IsPressed();
            AttackPressed = m_AttackAction.WasPressedThisFrame();
        }
        public Vector2 MovementInput { get; private set; }

        public bool FocusPressed { get; private set; }

        public bool JumpPressed { get; private set; }

        public bool EvadePressed { get; private set; }

        public bool SprintPressing { get; private set; }

        public bool CrouchPressed { get; private set; }

        public bool AttackPressed {  get; private set; }
    }
}
