using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    public class EventChannel<T> : ScriptableObject
    {
        readonly HashSet<EventListener<T>> observers = new();

        public void Invoke(T value)
        {
            foreach (var observer in observers)
            {
                observer.Raise(value);
            }
        }

        public void Register(EventListener<T> observer) => observers.Add(observer);
        public void Deregister(EventListener<T> observer) => observers.Remove(observer);
    }
    [CreateAssetMenu(menuName = "EventChannel/EventChannel")]
    public class EventChannel : EventChannel<Empty> { }
    public readonly struct Empty { }
}