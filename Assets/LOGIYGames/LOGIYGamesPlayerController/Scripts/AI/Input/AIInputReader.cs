using UnityEngine;
using System.Collections;
using LOGIYGames.CharacterCore;
using LOGIYGames.AI;

namespace LOGIYGames.Scripts.AI
{
	public class AIInputReader: ICharacterInputReader
	{
        AIBrain Driver;
        public AIInputReader(AIBrain Driver)
        {
            this.Driver = Driver;
        }

        public CharacterInput GetInput()
        {
            return Driver.Input;
        }
    }
}