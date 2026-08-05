using Unity.Cinemachine;
using UnityEngine;

namespace LOGIYGames
{
    public class CameraDutchModule : MonoBehaviour
    {
        private CinemachineCamera cinemachineCamera;

        PlayerInputReader inputReader;

        [SerializeField] private float maxLean = 8f;
        [SerializeField] private float smooth = 8f;

        private float currentLean;
        private void Start()
        {
            cinemachineCamera = GetComponent<CinemachineCamera>();
            inputReader = PlayerManager.Instance.PlayerInputReader;
        }
        void Update()
        {
            // A/D или Input System
            float horizontal = inputReader.GetInput().MovementInput.x;

            float targetLean = -horizontal * maxLean;

            currentLean = Mathf.Lerp(currentLean, targetLean, smooth * Time.deltaTime);

            var lens = cinemachineCamera.Lens;
            lens.Dutch = currentLean;
            cinemachineCamera.Lens = lens;
        }
    }
}
