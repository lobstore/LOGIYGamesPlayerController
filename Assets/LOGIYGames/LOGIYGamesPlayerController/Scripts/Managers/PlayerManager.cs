using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField] Character InitCharacter;

        [field: SerializeField] public IControllable CurrentControllable { get; private set; }
        [field: SerializeField] public PlayerMovementInputReader PlayerInputReader { get; private set; }
        private void Start()
        {
            PlayerInputReader?.Enable();
            SetCharacter(InitCharacter);
        }
        public void SetCharacter(IControllable character)
        {
            CurrentControllable = character;
            CurrentControllable?.SetInputReader(PlayerInputReader);
        }
    }
}
