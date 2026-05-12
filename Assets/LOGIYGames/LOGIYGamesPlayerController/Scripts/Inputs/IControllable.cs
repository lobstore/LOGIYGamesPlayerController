using UnityEngine;

namespace LOGIYGames.CharacterCore
{
    public interface IControllable
    {
        Transform CameraLookAt { get; }
        Transform CameraFollow { get; }
        void TakeControl(ICharacterInputReader inputReader);
        void ReleaseControl();
    }
}
