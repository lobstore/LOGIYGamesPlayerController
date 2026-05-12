using UnityEngine;



namespace LOGIYGames.CharacterCore
{
    public class NoneInput : ICharacterInputReader
    {
        public CharacterInput GetInput()
        {
            return new CharacterInput();
        }
    }
}
