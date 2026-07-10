using R3;
using System;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class Stamina
    {
        public SerializableReactiveProperty<float> Current = new();
        public SerializableReactiveProperty<float> Max = new();
    }
}
