using LOGIYGames.CharacterCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerMovementInputReader : IMovementInputReader
    {

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
        public PlayerMovementInputReader(InputActionAsset InputActions )
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

        public Vector2 MovementInput => m_MoveAction.ReadValue<Vector2>();

        public bool FocusPressed => m_FocusAction.IsPressed();

        public bool JumpPressed => m_JumpAction.WasPressedThisFrame();

        public bool EvadePressed => m_EvadeAction.WasPressedThisFrame();

        public bool SprintPressing => m_SprintAction.IsPressed();

        public bool CrouchPressed => m_CrouchAction.IsPressed();

        public bool AttackPressed => m_AttackAction.WasPressedThisFrame();
    }
}
