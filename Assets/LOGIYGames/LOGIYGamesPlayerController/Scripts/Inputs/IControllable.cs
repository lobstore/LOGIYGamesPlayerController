using Unity.Cinemachine;
using UnityEngine;

namespace LOGIYGames.CharacterCore
{
    public interface IControllable
    {
        CameraTarget TPVCameraTarget { get; }
        void UpdateInput(CharacterInput input);
    }
}
