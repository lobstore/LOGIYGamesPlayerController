using LOGIYGames.CharacterCore;
using R3;
using System;
using System.Collections.Generic;
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
        private PlayerProfilePresenter profilePresenter;

        [SerializeField] private GameObject abilityIconPrefab;
        [SerializeField] private RectTransform abilitiesContainer;

        private List<PlayerAbilityPresenter> abilityPresenters = new();
        private List<PlayerAbilityView> abilitiesViews = new();

        private List<PlayerEffectPresenter> effectsPresenters = new();
        private List<PlayerEffectView> effectsViews = new();

        [SerializeField] private GameObject effectIconPrefab;
        [SerializeField] private RectTransform effectsContainer;

        private ReactiveProperty<string> Name = new();

        IDisposable subscription;

        protected override void Awake()
        {
            base.Awake();
            PlayerInputReader = new(InputActions);
            OnCharacterChanged.AddListener((newChar) =>
            {
                UpdateProfileView(newChar);
                UpdateAbilitiesViews(newChar);
                subscription?.Dispose();
                subscription = newChar.EffectSystem.OnContinuousEffectsChanged.Subscribe((effects) =>
                {
                    UpdateEffectsViews(effects);
                });
                UpdateEffectsViews(newChar.EffectSystem.Effects);

            });
        }

        private void UpdateProfileView(Character newChar)
        {
            profilePresenter?.Dispose();
            Name.Value = newChar.name;
            profilePresenter = new PlayerProfilePresenter(newChar.HealthController.Health, newChar.StaminaController.Stamina, Name, profileView);
        }

        private void UpdateAbilitiesViews(Character newChar)
        {
            for (int i = 0; i < abilitiesContainer.childCount; i++)
            {
                Destroy(abilitiesContainer.GetChild(i).gameObject);
            }
            if (abilityPresenters.Count > 0)
            {
                abilityPresenters.Clear();
            }
            if (abilitiesViews.Count > 0)
            {
                foreach (var item in abilitiesViews)
                {
                    Destroy(item);
                }
                abilitiesViews.Clear();
            }

            if (newChar.GetComponent<AbilityController>().Abilities.Count > 0)
            {
                foreach (var item in newChar.GetComponent<AbilityController>().Abilities)
                {
                    var obj = Instantiate(abilityIconPrefab);
                    obj.transform.SetParent(abilitiesContainer, true);
                    obj.transform.localScale = Vector3.one;
                    var view = obj.GetComponent<PlayerAbilityView>();
                    abilitiesViews.Add(view);
                    abilityPresenters.Add(new PlayerAbilityPresenter(item, view));
                }
            }
        }

        private void UpdateEffectsViews(IReadOnlyList<RuntimeEffect> effects)
        {

            for (int i = 0; i < effectsContainer.childCount; i++)
            {
                Destroy(effectsContainer.GetChild(i).gameObject);
            }
            if (effectsPresenters.Count > 0)
            {
                effectsPresenters.Clear();
            }
            if (effectsViews.Count > 0)
            {
                foreach (var item in effectsViews)
                {
                    Destroy(item);
                }
                effectsViews.Clear();
            }

            if (effects.Count > 0)
            {
                foreach (var item in effects)
                {
                    if (item.Data.Icon == null) return;
                    var obj = Instantiate(effectIconPrefab);
                    obj.transform.SetParent(effectsContainer, true);
                    obj.transform.localScale = Vector3.one;
                    var view = obj.GetComponent<PlayerEffectView>();
                    effectsViews.Add(view);
                    effectsPresenters.Add(new PlayerEffectPresenter(item, view));
                }
            }
        }

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
