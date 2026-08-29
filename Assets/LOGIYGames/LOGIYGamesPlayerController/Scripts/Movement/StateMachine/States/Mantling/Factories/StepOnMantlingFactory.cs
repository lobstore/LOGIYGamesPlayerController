using LOGIYGames.CharacterCore;
using UnityEngine;

namespace LOGIYGames
{
    [CreateAssetMenu(fileName ="StepOnMantlingFactory", menuName = "Mantling/Factories/StepOn")]
    public class StepOnMantlingFactory : MantlingFactory
    {
        public override MantlingStrategy Create(Actor chr)
        {
            return new StepOnMantling(chr, mantlingData);
        }
    }
}
