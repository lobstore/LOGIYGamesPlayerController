using Unity.Cinemachine;
using UnityEngine;

namespace LOGIYGames.CharacterCore
{
    public interface IControllable
    {
        CameraTarget CameraTarget { get; }
        void SetInput(ICharacterInputReader inputReader);
    }
}
