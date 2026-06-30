using LOGIYGames.CharacterCore;

namespace LOGIYGames
{


    public sealed class DamageContext
    {
        public Character Source;
        public Character Target;

        public float Damage;

        public DamageType DamageType;

        public bool IsCritical;

        public bool Cancelled;
    }

}
