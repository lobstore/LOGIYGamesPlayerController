using LOGIYGames.CharacterCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField] Character InitCharacter;
        [SerializeField] InputActionAsset InputActions;
        public Character CurrentCharacter { get; private set; }
        [field: SerializeField] public PlayerMovementInputReader PlayerInputReader { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            PlayerInputReader = new(InputActions);
        }
        private void Start()
        {
            PlayerInputReader?.Enable();
            SetPlayerControlOnCharacter(InitCharacter);
        }
        private void Update()
        {
            Aim();
            UpdateStrategies();
        }

        private void UpdateStrategies()
        {
            switch (CameraManager.Instance.CameraPerspectiveType)
            {
                case CameraPerspectiveType.FirstPerson:
                    CurrentCharacter.DefaultMovementStrategy = new CameraRelativeMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new CameraAlongRotation();
                    break;
                case CameraPerspectiveType.ThirdPersonFreeLook:
                    CurrentCharacter.DefaultMovementStrategy = new CameraRelativeMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new CameraRelativeRotation(CurrentCharacter);
                    break;
                case CameraPerspectiveType.ThirdPersonLookForward:
                    CurrentCharacter.DefaultMovementStrategy = new CameraRelativeMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new CameraAlongRotation();
                    break;
                case CameraPerspectiveType.Top_Down:
                    CurrentCharacter.DefaultMovementStrategy = new CameraRelativeMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new CameraRelativeRotation(CurrentCharacter);
                    break;
                default:
                    break;
            }
        }

        private void Aim()
        {
            if (CurrentCharacter.Input.FocusPressed && CameraManager.Instance.CameraPerspectiveType == CameraPerspectiveType.ThirdPersonFreeLook)
            {
                CameraManager.Instance.CameraPerspectiveType = CameraPerspectiveType.ThirdPersonLookForward;
            }
            else if(!CurrentCharacter.Input.FocusPressed && CameraManager.Instance.CameraPerspectiveType == CameraPerspectiveType.ThirdPersonLookForward)
            {
                CameraManager.Instance.CameraPerspectiveType = CameraPerspectiveType.ThirdPersonFreeLook;
            }
        }
        public void SetPlayerControlOnCharacter(Character character)
        {
            CurrentCharacter?.ReleaseControl();
            CurrentCharacter = character;
            UpdateStrategies();

            CurrentCharacter?.TakeControl(PlayerInputReader);

            CameraManager.Instance.SetTargetTo(CurrentCharacter.CameraFollow, CurrentCharacter.CameraLookAt);
        }
    }
}
