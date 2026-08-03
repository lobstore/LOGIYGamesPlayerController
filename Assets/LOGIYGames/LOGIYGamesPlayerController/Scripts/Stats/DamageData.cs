using System;
namespace LOGIYGames
{
    [Serializable]
    public struct DamageData
    {
        public float Amount;
        public ModifierType ModifierType;
        public DamageType Type;
    }
}
