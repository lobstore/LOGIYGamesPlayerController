using LOGIYGames.CharacterCore;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerManager : PersistentSingleton<PlayerManager>
    {
        [SerializeField] Actor InitCharacter;
        [SerializeField] InputActionAsset InputActions;
        public UnityEvent<Actor> OnCharacterChanged = new();
        public Actor CurrentCharacter { get; private set; }
        public PlayerInputReader PlayerInput { get; private set; }

        [SerializeField] private CameraPerspectiveType currentControlType;
        private bool isLockOn;

        public CameraPerspectiveType CurrentCameraPerspectiveType
        {
            get { return currentControlType; }
            set
            {
                currentControlType = value;
                UpdateStrategies();
            }
        }
        protected override void Awake()
        {
            base.Awake();
            PlayerInput = new(InputActions, Camera.main.transform);
        }

        private void Start()
        {
            SetPlayerControlOnCharacter(InitCharacter);
            PlayerInput?.Enable();
        }

        private void Update()
        {
            CharacterInput input = PlayerInput.GetInput();
            CurrentCharacter.UpdateInput(input);
            if (CurrentCharacter.TargetingController.HasTarget)
            {
                if (CurrentCharacter.Input.LockPressed)
                {
                    LockOnToggle();
                }
            }
            else
            {
                isLockOn = false;
            }
        }
        private void LateUpdate()
        {
            
            if (isLockOn)
            {
                SetLockOn();
            }
            else
            {
                UpdateStrategies();

            }
        }
        private void LockOnToggle()
        {
            if (isLockOn)
            {
                isLockOn = false;
            }
            else
            {
                isLockOn = true;
            }
        }
        public void SetLockOn()
        {
            CurrentCharacter.DefaultMovementStrategy = new StrafeMovement(CurrentCharacter);
            CurrentCharacter.DefaultRotationStrategy = new TargetLockRotation(CurrentCharacter);
        }
        private void UpdateStrategies()
        {
            switch (currentControlType)
            {
                case CameraPerspectiveType.FirstPerson:
                    CurrentCharacter.DefaultMovementStrategy = new StrafeMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new MousePlanarRotation(CurrentCharacter);
                    PlayerInput = new(InputActions, CurrentCharacter.transform);
                    break;
                case CameraPerspectiveType.ThirdPersonFreeLook:
                    CurrentCharacter.DefaultMovementStrategy = new CharacterForwardMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new CharacterRelativeRotation(CurrentCharacter);
                    PlayerInput = new(InputActions, Camera.main.transform);
                    break;
                case CameraPerspectiveType.ThirdPersonLookForward:
                    CurrentCharacter.DefaultMovementStrategy = new StrafeMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new CharacterForwardRotation(CurrentCharacter);
                    PlayerInput = new(InputActions, Camera.main.transform);
                    break;
                case CameraPerspectiveType.Top_Down:
                    CurrentCharacter.DefaultMovementStrategy = new CharacterForwardMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new CharacterRelativeRotation(CurrentCharacter);
                    PlayerInput = new(InputActions, Camera.main.transform);
                    break;
                default:
                    break;
            }
        }
        public void SetPlayerControlOnCharacter(Actor character)
        {
            CurrentCharacter = character;
            CurrentCharacter.ResetStrategies();
            OnCharacterChanged?.Invoke(CurrentCharacter);
        }
    }
}
