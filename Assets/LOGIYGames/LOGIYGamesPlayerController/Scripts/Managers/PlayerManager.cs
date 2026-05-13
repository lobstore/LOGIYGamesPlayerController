using LOGIYGames.CharacterCore;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField] Character InitCharacter;
        [SerializeField] InputActionAsset InputActions;
        public Character CurrentCharacter { get; private set; }
        [SerializeField] CinemachineTargetGroup TargetGroup;
        CinemachineTargetGroup.Target c_Target = new();
        public bool IsLockedOn { get; private set; }
        [field: SerializeField] public PlayerMovementInputReader PlayerInputReader { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            PlayerInputReader = new(InputActions);
        }
        private void Start()
        {
            PlayerInputReader?.Enable();
            c_Target.Radius = 4f;
            c_Target.Weight = 4f;
            SetPlayerControlOnCharacter(InitCharacter);
        }
        private void Update()
        {
            // Aim();
            if (CurrentCharacter.Input.FocusPressed && !IsLockedOn)
            {
                LockTarget();
            }
            else if (!CurrentCharacter.Input.FocusPressed && IsLockedOn)
            {
                UnlockTarget();
            }
            UpdateStrategies();
        }
        private void LockTarget()
        {
            if (!CurrentCharacter.Targeting.HasTarget) return;  
            c_Target.Object = CurrentCharacter.Targeting.CurrentTarget;
            TargetGroup.Targets.Add(c_Target);
            CameraManager.Instance.SetTargetTo(CurrentCharacter.CameraFollow, TargetGroup.transform);
            IsLockedOn = true;
        }
        private void UnlockTarget()
        {
            TargetGroup.Targets.Remove(c_Target);

            CameraManager.Instance.SetTargetTo(CurrentCharacter.CameraFollow, CurrentCharacter.CameraLookAt);
            IsLockedOn = false;
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
            TargetGroup.Targets.Clear();
            TargetGroup.Targets.Add(new CinemachineTargetGroup.Target { Object = CurrentCharacter.CameraFollow, Radius = 0.2f, Weight = 10 });

            CameraManager.Instance.SetTargetTo(CurrentCharacter.CameraFollow, CurrentCharacter.CameraLookAt);
        }
    }
}
