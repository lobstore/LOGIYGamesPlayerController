using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField] Character InitCharacter;
        private bool IsAiControl;

        [field: SerializeField] public IControllable CurrentControllable {  get; private set; }
        //[field: SerializeField] public InputReader InputReader {  get; private set; }
        [field: SerializeField] public PlayerMovementInputReader PlayerInputReader {  get; private set; }
        private void Start()
        {
            //InputReader?.EnableAllInputs();
            PlayerInputReader?.Enable();
            SetCharacter(InitCharacter);
        }
        public void SetCharacter (IControllable character)
        {
            CurrentControllable?.ReleaseControl();
            CurrentControllable = character;
            CurrentControllable?.TakeControl(PlayerInputReader);

        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SwitchControl();
            }
        }

        public void SwitchControl()
        {
            IsAiControl = !IsAiControl;
            if (IsAiControl)
            {
                CurrentControllable?.ReleaseControl();
            }
            else
            {
                CurrentControllable?.TakeControl(PlayerInputReader);
            }
        }
    }
}
