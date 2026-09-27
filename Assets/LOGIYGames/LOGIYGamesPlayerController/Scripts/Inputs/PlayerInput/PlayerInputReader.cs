using LOGIYGames.CharacterCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerInputReader : ICharacterInputReader
    {

        InputActionMap CharacterActionMap;
        List<InputAction> movementActions = new();
        InputAction m_MoveAction;
        InputAction m_JumpAction;
        InputAction m_EvadeAction;
        InputAction m_CrouchAction;
        InputAction m_SprintAction;
        InputAction m_FocusAction;
        InputAction m_AttackAction;
        InputAction m_InteractAction;
        InputAction m_AbilityAction;
        Transform Pivot;


        public PlayerInputReader(InputActionAsset InputActions, Transform pivot)
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
            m_AbilityAction = CharacterActionMap.FindAction("Ability");
            Pivot = pivot;
            movementActions.Add(m_MoveAction);
            movementActions.Add(m_JumpAction);
            movementActions.Add(m_EvadeAction);
            movementActions.Add(m_CrouchAction);
            movementActions.Add(m_SprintAction);
            movementActions.Add(m_AttackAction);

        }
        public void DisableMovement()
        {
            foreach (var movement in movementActions)
            {
                movement.Disable();
            }
        }
        public void EnableMovement()
        {
            foreach (var movement in movementActions)
            {
                movement.Enable();
            }
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
            input.CrouchPressed = m_CrouchAction.WasPressedThisFrame();
            input.CrouchPressing = m_CrouchAction.IsPressed();
            input.AbilityPressed = m_AbilityAction.WasPressedThisFrame();
            input.InteractPressed = m_InteractAction.WasReleasedThisFrame();
            input.InteractHeld = m_InteractAction.WasPerformedThisFrame();
            input.LookForward = Pivot.forward;
            input.LookRight = Pivot.right;
            return input;
        }

    }
}
