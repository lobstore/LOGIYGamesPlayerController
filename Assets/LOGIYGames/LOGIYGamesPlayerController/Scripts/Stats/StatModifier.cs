using System;

namespace LOGIYGames
{
    [Serializable]
    public class StatModifier
    {
        public ModifierType Type;
        public float Value;
        public object Source;

        public StatModifier(ModifierType type, float value, object source)
        {
            Type = type;
            Value = value;
            Source = source;
        }
    }

}
