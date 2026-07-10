using LOGIYGames.CharacterCore;
using R3;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerManager : PersistentSingleton<PlayerManager>
    {
        [SerializeField] Character InitCharacter;
        [SerializeField] InputActionAsset InputActions;

        public UnityEvent<Character> OnCharacterChanged = new();
        public Character CurrentCharacter { get; private set; }

        //public readonly UnityEvent<bool> OnTargetLocked = new();
        //public CinemachineTargetGroup TargetGroup { get; private set; }
        //CinemachineTargetGroup.Target c_Target = new();
        //private bool isLockedOn;

        //public bool IsLockedOn { get { return isLockedOn; } private set { isLockedOn = value; OnTargetLocked.Invoke(isLockedOn); } }
        [field: SerializeField] public PlayerInputReader PlayerInputReader { get; private set; }

        [SerializeField] private PlayerProfileView profileView;
        [SerializeField] private GameObject abilityIconPrefab;
        private PlayerProfilePresenter profilePresenter;
        [SerializeField] private RectTransform skillsContainer;
        private List<PlayerSkillPresenter> skillPresenters = new();
        private List<PlayerAbilityView> skillViews = new();
        private ReactiveProperty<string> Name = new();
        protected override void Awake()
        {
            base.Awake();
            PlayerInputReader = new(InputActions);
            OnCharacterChanged.AddListener((newChar) =>
            {

                profilePresenter?.Dispose();
                Name.Value = newChar.name;
                profilePresenter = new PlayerProfilePresenter(newChar.Health, newChar.StaminaController.Stamina, Name, profileView);

             //   UpdateAbilityViews(newChar);
            });
        }

        //private void UpdateAbilityViews(Character newChar)
        //{
        //    for (int i = 0; i < skillsContainer.childCount; i++)
        //    {
        //        Destroy(skillsContainer.GetChild(i));
        //    }
        //    if (skillPresenters.Count > 0)
        //    {
        //        skillPresenters.Clear();
        //    }
        //    if (skillViews.Count > 0)
        //    {
        //        foreach (var item in skillViews)
        //        {
        //            Destroy(item);
        //        }
        //        skillViews.Clear();
        //    }

        //    if (newChar.AbilityController.Abilities.Count > 0)
        //    {
        //        foreach (var item in newChar.AbilityController.Abilities)
        //        {
        //            var obj = Instantiate(abilityIconPrefab);
        //            obj.transform.SetParent(skillsContainer, true);
        //            obj.transform.localScale = Vector3.one;
        //            var view = obj.GetComponent<PlayerAbilityView>();
        //            skillViews.Add(view);
        //            skillPresenters.Add(new PlayerSkillPresenter(item, view));
        //        }
        //    }
        //}

        private void Start()
        {

            SetPlayerControlOnCharacter(InitCharacter);
            PlayerInputReader?.Enable();


            //c_Target.Object = CurrentCharacter.Targeting.CurrentTarget;
            //if (TargetGroup == null)
            //{
            //    TargetGroup = new GameObject("CameraTargets_Runtime").AddComponent<CinemachineTargetGroup>();

            //}
            //TargetGroup.Targets.Clear();
            //TargetGroup.Targets.Add(new CinemachineTargetGroup.Target { Object = CurrentCharacter.CameraTarget.CameraFollow, Radius = 0.2f, Weight = 10 });
            //TargetGroup.Targets.Add(c_Target);
            //c_Target.Radius = 4f;
            //c_Target.Weight = 4f;


        }
        private void Update()
        {
            CharacterInput input = PlayerInputReader.GetInput();
            CurrentCharacter.UpdateInput(input);

            //if (input.FocusPressed && !IsLockedOn)
            //{
            //    LockOnTarget();
            //}
            //else if (!input.FocusPressed && IsLockedOn)
            //{
            //    LockOffTarget();
            //}

            UpdateStrategies();
        }
        //private void LockOnTarget()
        //{
        //    if (!CurrentCharacter.Targeting.HasTarget) return;




        //    IsLockedOn = true;
        //}
        //private void LockOffTarget()
        //{
        //    CameraManager.Instance.SetTargetTo(CurrentCharacter.CameraTarget.CameraFollow, CurrentCharacter.CameraTarget.CameraLookAt);
        //    IsLockedOn = false;
        //}
        private void UpdateStrategies()
        {
            switch (CameraManager.Instance.CurrentCameraPerspectiveType)
            {
                case CameraPerspectiveType.FirstPerson:
                    CurrentCharacter.DefaultMovementStrategy = new PlanarInputMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new FirstPersonPlanarRotation(CurrentCharacter);
                    break;
                case CameraPerspectiveType.ThirdPersonFreeLook:
                    CurrentCharacter.DefaultMovementStrategy = new CharacterForwardMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new LookRelativeRotation(CurrentCharacter);
                    break;
                case CameraPerspectiveType.ThirdPersonLookForward:
                    CurrentCharacter.DefaultMovementStrategy = new PlanarInputMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new FirstPersonPlanarRotation(CurrentCharacter);
                    break;
                case CameraPerspectiveType.Top_Down:
                    CurrentCharacter.DefaultMovementStrategy = new CharacterForwardMovement(CurrentCharacter);
                    CurrentCharacter.DefaultRotationStrategy = new LookRelativeRotation(CurrentCharacter);
                    break;
                default:
                    break;
            }
        }
        public void SetPlayerControlOnCharacter(Character character)
        {
            CurrentCharacter = character;
            UpdateStrategies();
            CurrentCharacter.ResetStrategies();
            CameraManager.Instance.SetTargetTo(CurrentCharacter.CameraTarget.CameraFollow, CurrentCharacter.CameraTarget.CameraLookAt);
            OnCharacterChanged?.Invoke(CurrentCharacter);
        }
    }
}
