using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    /// <summary>
    /// Debug component to diagnose rotation issues
    /// </summary>
    public class CharacterRotationDebugger : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Character character;
        [SerializeField] private MovementStateDriver stateDriver;
        
        [Header("Debug Info")]
        [SerializeField] private bool hasMovementInput;
        [SerializeField] private Vector2 movementInput;
        [SerializeField] private float turnSmoothTime;
        [SerializeField] private string currentRotationStrategy;
        [SerializeField] private Quaternion targetRotation;
        [SerializeField] private Quaternion currentRotation;
        
        [Header("Camera Info")]
        [SerializeField] private CameraPerspectiveType cameraPerspective;
        [SerializeField] private bool blockPressed;
        
        private void Update()
        {
            if (character == null)
            {
                character = GetComponent<Character>();
            }
            
            if (stateDriver == null)
            {
                stateDriver = GetComponent<MovementStateDriver>();
            }
            
            // Update debug info
            hasMovementInput = character.MovementInput.magnitude > 0.01f;
            movementInput = character.MovementInput;
            turnSmoothTime = character.TurnSmoothTime;
            
            if (stateDriver != null && stateDriver.StateMachine != null)
            {
                var currentState = stateDriver.StateMachine.CurrentNode?.State as BaseState;
                if (currentState != null)
                {
                    currentRotationStrategy = currentState.CurrentRotationStrategy?.GetType().Name ?? "None";
                    targetRotation = currentState.CurrentRotationStrategy?.GetRotation() ?? Quaternion.identity;
                }
            }
            
            currentRotation = transform.rotation;
            
            if (CameraManager.Instance != null)
            {
                cameraPerspective = CameraManager.Instance.CameraPerspectiveType;
            }
            
            // Log warning if rotation seems stuck
            if (turnSmoothTime <= 0)
            {
                Debug.LogWarning("[CharacterRotationDebugger] TurnSmoothTime is 0 or negative! Rotation will be instant.");
            }
        }
        
        private void OnGUI()
        {
            if (!character) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 400, 300));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label($"=== Character Rotation Debug ===");
            GUILayout.Label($"Movement Input: {movementInput} (has: {hasMovementInput})");
            GUILayout.Label($"Turn Smooth Time: {turnSmoothTime}");
            GUILayout.Label($"Current Rotation Strategy: {currentRotationStrategy}");
            GUILayout.Label($"Target Rotation Y: {targetRotation.eulerAngles.y:F1}");
            GUILayout.Label($"Current Rotation Y: {currentRotation.eulerAngles.y:F1}");
            GUILayout.Label($"Camera Perspective: {cameraPerspective}");
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
