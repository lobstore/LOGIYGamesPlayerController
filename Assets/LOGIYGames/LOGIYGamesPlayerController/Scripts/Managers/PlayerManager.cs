using LOGIYGames.CharacterCore;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField] Character InitCharacter;
        [SerializeField] InputActionAsset InputActions;
        public Character CurrentCharacter { get; private set; }

        public readonly UnityEvent<bool> OnTargetLocked = new();
        public CinemachineTargetGroup TargetGroup {  get; private set; }
        CinemachineTargetGroup.Target c_Target = new();
        private bool isLockedOn;

        public bool IsLockedOn { get { return isLockedOn; } private set { isLockedOn = value; OnTargetLocked.Invoke(isLockedOn); } }
        [field: SerializeField] public PlayerMovementInputReader PlayerInputReader { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            PlayerInputReader = new(InputActions);
        }
        private void Start()
        {


            SetPlayerControlOnCharacter(InitCharacter);
            c_Target.Object = CurrentCharacter.Targeting.CurrentTarget;
            if (TargetGroup == null)
            {
                TargetGroup = new GameObject("CameraTargets_Runtime").AddComponent<CinemachineTargetGroup>();

            }
            TargetGroup.Targets.Clear();
            TargetGroup.Targets.Add(new CinemachineTargetGroup.Target { Object = CurrentCharacter.CameraFollow, Radius = 0.2f, Weight = 10 });
            TargetGroup.Targets.Add(c_Target);
            PlayerInputReader?.Enable();
            c_Target.Radius = 4f;
            c_Target.Weight = 4f;
        }
        private void Update()
        {
            // Aim();
            if (CurrentCharacter.IsAimig && !IsLockedOn)
            {
                LockTarget();
            }
            else if (!CurrentCharacter.IsAimig && IsLockedOn)
            {
                UnlockTarget();
            }
            UpdateStrategies();
        }
        private void LockTarget()
        {
            if (!CurrentCharacter.Targeting.HasTarget) return;




            IsLockedOn = true;
        }
        private void UnlockTarget()
        {
            //TargetGroup.Targets.Remove(c_Target);

            CameraManager.Instance.SetTargetTo(CurrentCharacter.CameraFollow, CurrentCharacter.CameraLookAt);
            IsLockedOn = false;
        }
        private void UpdateStrategies()
        {
            switch (CameraManager.Instance.CurrentCameraPerspectiveType)
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
                case CameraPerspectiveType.LockOn:
                    CurrentCharacter.DefaultMovementStrategy = new CameraRelativeMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new TargetLockRotation(CurrentCharacter);
                    break;
                default:
                    break;
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
