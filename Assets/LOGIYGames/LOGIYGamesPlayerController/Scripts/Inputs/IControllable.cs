using UnityEngine;

namespace LOGIYGames.CharacterCore
{
    public interface IControllable
    {
        Transform CameraLookAt { get; }
        Transform CameraFollow { get; }
        void UpdateInput(CharacterInput input);
    }
}
