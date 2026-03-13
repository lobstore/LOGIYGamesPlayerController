using System;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    public interface IEventDispatcher
    {
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class;
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class;
        void Publish<TEvent>(TEvent eventData) where TEvent : class;
    }
    public class EventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<Type, Delegate> _subscriptions = new();

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class
        {
            var eventType = typeof(TEvent);
            if (_subscriptions.TryGetValue(eventType, out var existingDelegate))
            {
                _subscriptions[eventType] = Delegate.Combine(existingDelegate, handler);
            }
            else
            {
                _subscriptions[eventType] = handler;
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class
        {
            var eventType = typeof(TEvent);
            if (_subscriptions.TryGetValue(eventType, out var existingDelegate))
            {
                var newDelegate = Delegate.Remove(existingDelegate, handler);
                if (newDelegate == null)
                    _subscriptions.Remove(eventType);
                else
                    _subscriptions[eventType] = newDelegate;
            }
        }

        public void Publish<TEvent>(TEvent eventData) where TEvent : class
        {
            var eventType = typeof(TEvent);
            if (_subscriptions.TryGetValue(eventType, out var existingDelegate))
            {
                if (existingDelegate is Action<TEvent> typedDelegate)
                {
                    // Безопасный вызов
                    try
                    {
                        typedDelegate.Invoke(eventData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e); // Логирование ошибки подписчика
                    }
                }
            }
        }

        public void Clear()
        {
            _subscriptions.Clear();
        }
    }
}
