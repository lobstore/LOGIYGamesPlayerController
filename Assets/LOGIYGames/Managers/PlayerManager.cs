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
        public PlayerInputReader InputReader { get; private set; }

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
            InputReader = new(InputActions, Camera.main.transform);
        }

        private void Start()
        {
            SetPlayerControlOnCharacter(InitCharacter);
            InputReader?.Enable();
        }

        private void Update()
        {
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
            CurrentCharacter.MovementStrategy = new StrafeMovement(CurrentCharacter);
            CurrentCharacter.RotationStrategy = new TargetLockRotation(CurrentCharacter);
        }
        private void UpdateStrategies()
        {
            switch (currentControlType)
            {
                case CameraPerspectiveType.FirstPerson:
                    CurrentCharacter.MovementStrategy = new StrafeMovement(CurrentCharacter);
                    CurrentCharacter.RotationStrategy = new MousePlanarRotation(CurrentCharacter);
                    InputReader = new(InputActions, CurrentCharacter.transform);
                    break;
                case CameraPerspectiveType.ThirdPersonFreeLook:
                    CurrentCharacter.MovementStrategy = new CharacterForwardMovement(CurrentCharacter);
                    CurrentCharacter.RotationStrategy = new CharacterRelativeRotation(CurrentCharacter);
                    InputReader = new(InputActions, Camera.main.transform);
                    break;
                case CameraPerspectiveType.ThirdPersonLookForward:
                    CurrentCharacter.MovementStrategy = new StrafeMovement(CurrentCharacter);
                    CurrentCharacter.RotationStrategy = new CharacterForwardRotation(CurrentCharacter);
                    InputReader = new(InputActions, Camera.main.transform);
                    break;
                case CameraPerspectiveType.Top_Down:
                    CurrentCharacter.MovementStrategy = new CharacterForwardMovement(CurrentCharacter);
                    CurrentCharacter.RotationStrategy = new CharacterRelativeRotation(CurrentCharacter);
                    InputReader = new(InputActions, Camera.main.transform);
                    break;
                default:
                    break;
            }
        }
        public void SetPlayerControlOnCharacter(Actor character)
        {
            CurrentCharacter = character;
            CurrentCharacter.SetInput(InputReader);
            OnCharacterChanged?.Invoke(CurrentCharacter);
        }
    }
}
