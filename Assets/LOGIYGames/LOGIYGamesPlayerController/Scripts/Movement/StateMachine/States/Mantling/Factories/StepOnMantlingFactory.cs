using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName ="StepOnMantlingFactory", menuName = "Mantling/Factories/StepOn")]
    public class StepOnMantlingFactory : MantlingFactory
    {
        public override MantlingStrategy Create(CharacterModule chr)
        {
            return new StepOnMantling(chr, mantlingData);
        }
    }
}
