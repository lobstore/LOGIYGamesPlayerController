using LOGIYGames.CharacterCore;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField] CharacterModule InitCharacter;
        [SerializeField] InputActionAsset InputActions;

        public UnityEvent<CharacterModule> OnCharacterChanged = new();
        public CharacterModule CurrentCharacter { get; private set; }

        public readonly UnityEvent<bool> OnTargetLocked = new();
        public CinemachineTargetGroup TargetGroup { get; private set; }
        CinemachineTargetGroup.Target c_Target = new();
        private bool isLockedOn;

        public bool IsLockedOn { get { return isLockedOn; } private set { isLockedOn = value; OnTargetLocked.Invoke(isLockedOn); } }
        [field: SerializeField] public PlayerInputReader PlayerInputReader { get; private set; }

        [SerializeField] private PlayerHealthView healthView;
        private PlayerHealthPresenter healthPresenter;

        protected override void Awake()
        {
            base.Awake();
            PlayerInputReader = new(InputActions);
            OnCharacterChanged.AddListener((newChar) =>
            {
                healthPresenter?.Dispose();
                healthPresenter = new PlayerHealthPresenter(newChar.GetComponent<HealthModule>(), healthView);
            });
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
            CharacterInput input = PlayerInputReader.GetInput();
            CurrentCharacter.UpdateInput(input);
            if (input.FocusPressed && !IsLockedOn)
            {
                LockOnTarget();
            }
            else if (!input.FocusPressed && IsLockedOn)
            {
                LockOffTarget();
            }
            UpdateStrategies();
        }
        private void LockOnTarget()
        {
            if (!CurrentCharacter.Targeting.HasTarget) return;




            IsLockedOn = true;
        }
        private void LockOffTarget()
        {
            CameraManager.Instance.SetTargetTo(CurrentCharacter.CameraFollow, CurrentCharacter.CameraLookAt);
            IsLockedOn = false;
        }
        private void UpdateStrategies()
        {
            switch (CameraManager.Instance.CurrentCameraPerspectiveType)
            {
                case CameraPerspectiveType.FirstPerson:
                    CurrentCharacter.DefaultMovementStrategy = new PlanarMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new FirstPersonPlanarRotation(CurrentCharacter);
                    break;
                case CameraPerspectiveType.ThirdPersonFreeLook:
                    CurrentCharacter.DefaultMovementStrategy = new PlanarMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new ThirdPersonPlanarRotation(CurrentCharacter);
                    break;
                case CameraPerspectiveType.ThirdPersonLookForward:
                    CurrentCharacter.DefaultMovementStrategy = new PlanarMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new FirstPersonPlanarRotation(CurrentCharacter);
                    break;
                case CameraPerspectiveType.Top_Down:
                    CurrentCharacter.DefaultMovementStrategy = new PlanarMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new ThirdPersonPlanarRotation(CurrentCharacter);
                    break;
                default:
                    break;
            }
        }
        public void SetPlayerControlOnCharacter(CharacterModule character)
        {
            CurrentCharacter = character;
            UpdateStrategies();
            CurrentCharacter.ResetStrategies();
            CameraManager.Instance.SetTargetTo(CurrentCharacter.CameraFollow, CurrentCharacter.CameraLookAt);
            OnCharacterChanged?.Invoke(CurrentCharacter);
        }
    }
}
