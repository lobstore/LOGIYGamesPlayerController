using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames.CharacterCore
{
    public interface IControllable
    {
        IRotationStrategy RotationStrategy { get; set; }
        IMovementStrategy MovementStrategy { get; set; }
        Transform CameraLookAt { get; }
        Transform CameraFollow { get; }
        UnityEvent OnControlReleased { get; }
        void TakeControl(IMovementInputReader inputReader);
        void Release();
    }
}
