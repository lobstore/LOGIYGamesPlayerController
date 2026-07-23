using R3;
using System;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class Health
    {
        public SerializableReactiveProperty<float> Current = new();
        public SerializableReactiveProperty<float> Max = new();
    }
}
