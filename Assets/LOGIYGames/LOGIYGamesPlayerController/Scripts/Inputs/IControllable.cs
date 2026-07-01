using UnityEngine;

namespace LOGIYGames.CharacterCore
{
    public interface IControllable
    {
        CameraTarget CameraTarget { get; }
        void UpdateInput(CharacterInput input);
    }
}
