using LOGIYGames.CharacterCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerMovementInputReader : ICharacterInputReader
    {

        InputActionMap CharacterActionMap;

        InputAction m_MoveAction;
        InputAction m_JumpAction;
        InputAction m_EvadeAction;
        InputAction m_CrouchAction;
        InputAction m_SprintAction;
        InputAction m_FocusAction;
        InputAction m_AttackAction;
        InputAction m_InteractAction;
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
            m_InteractAction = CharacterActionMap.FindAction("Interact");
        }

        public void Enable()
        {
            CharacterActionMap.Enable();
        }
        public void Disable()
        {
            CharacterActionMap.Disable();
        }

        public CharacterInput GetInput()
        {
            CharacterInput input = new();

            input.MovementInput = m_MoveAction.ReadValue<Vector2>();
            input.FocusPressed = m_FocusAction.IsPressed();
            input.JumpPressed = m_JumpAction.WasPressedThisFrame();
            input.EvadePressed = m_EvadeAction.WasPressedThisFrame();
            input.SprintPressing = m_SprintAction.IsPressed();
            input.AttackPressed = m_AttackAction.WasPressedThisFrame();
            input.InteractPressed = m_InteractAction.WasPressedThisFrame();
            return input;
        }

    }
}
