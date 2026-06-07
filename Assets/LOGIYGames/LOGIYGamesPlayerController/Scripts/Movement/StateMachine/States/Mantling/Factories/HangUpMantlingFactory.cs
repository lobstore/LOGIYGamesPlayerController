using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName = "HangUpMantlingFactory", menuName = "Mantling/Factories/HangUp")]
    public class HangUpMantlingFactory : MantlingFactory
    {
        public override MantlingStrategy Create(CharacterModule chr)
        {
            return new HangUpMantling(chr, mantlingData);
        }
    }
}
