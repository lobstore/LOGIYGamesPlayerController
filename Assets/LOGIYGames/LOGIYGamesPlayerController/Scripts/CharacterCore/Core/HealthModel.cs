using LOGIYGames.Shared.Data;
using R3;
using System;
namespace LOGIYGames.CharacterCore
{
    [Serializable]
    public class HealthModel
    {
        public SerializableReactiveProperty<float> MaxHealth { get; private set; } = new();

        public SerializableReactiveProperty<float> CurrentHealth {  get; private set; } = new();

    }
}
