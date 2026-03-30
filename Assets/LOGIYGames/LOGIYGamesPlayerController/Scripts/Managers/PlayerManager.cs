using LOGIYGames.CharacterCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LOGIYGames
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField] Character InitCharacter;
        [SerializeField] InputActionAsset InputActions;
        public IControllable CurrentControllable { get; private set; }
        [field: SerializeField] public PlayerMovementInputReader PlayerInputReader { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            PlayerInputReader = new(InputActions);
        }
        private void Start()
        {
            PlayerInputReader?.Enable();
            SetCharacter(InitCharacter);
        }
        public void SetCharacter(IControllable character)
        {
            CurrentControllable?.Release();
            CurrentControllable = character;
            CurrentControllable?.TakeControl(PlayerInputReader);
            CurrentControllable.MovementStrategy = InitCharacter.DefaultMovementStrategy;
            CurrentControllable.RotationStrategy = InitCharacter.DefaultRotationStrategy;
            CameraManager.Instance.SetTargetTo(CurrentControllable.CameraFollow, CurrentControllable.CameraLookAt);
        }
    }
}
