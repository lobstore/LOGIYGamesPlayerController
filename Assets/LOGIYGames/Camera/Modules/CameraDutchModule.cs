using LOGIYGames.CharacterCore;
using LOGIYGames.Movement;
using Unity.Cinemachine;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace LOGIYGames
{
    public class CameraDutchModule : MonoBehaviour
    {
        private CinemachineCamera cinemachineCamera;

        PlayerInputReader inputReader;

        WallRunMovementState state;
        Actor actor;
        [SerializeField] private float maxLean = 8f;
        [SerializeField] private float smooth = 8f;

        private float currentLean;
        private void Start()
        {
            actor = PlayerManager.Instance.CurrentCharacter;
            state = actor.GetMovementState<WallRunMovementState>();
            cinemachineCamera = GetComponent<CinemachineCamera>();
            inputReader = PlayerManager.Instance.InputReader;
        }
        void Update()
        {
            float horizontal;

            float targetLean;
            if (state.IsActiveState)
            {
                horizontal = actor.Sensors.IsObstacleLegsLeft ? 1 : -1;
                targetLean = -horizontal * maxLean * 10;
            }
            else
            {
                horizontal = inputReader.GetInput().MovementInput.x;
                targetLean = -horizontal * maxLean;
            }
            
            // A/D или Input System


            currentLean = Mathf.Lerp(currentLean, targetLean, smooth * Time.deltaTime);
            cinemachineCamera.Lens.Dutch = currentLean;
        }
    }
}
