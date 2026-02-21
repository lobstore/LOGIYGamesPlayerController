using UnityEngine;
using UnityEngine.Events;
namespace LOGIYGames
{
    public class EventListener<T> : MonoBehaviour
    {
        [SerializeField] EventChannel<T> eventChannel;
        [SerializeField] UnityEvent<T> unityEvent;
        private void Awake()
        {
            eventChannel.Register(this);
        }
        private void OnDestroy()
        {
            eventChannel.Deregister(this);
        }
        public void Raise(T value)
        {
            unityEvent?.Invoke(value);
        }
    }

    public class EventListener : EventListener<Empty> { }
}