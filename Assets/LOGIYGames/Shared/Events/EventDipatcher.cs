using LOGIYGames.Shared.Character.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    public interface IEventDispatcher
    {
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : EventBase;
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : EventBase;
        void Publish<TEvent>(TEvent eventData) where TEvent : EventBase;
    }
    public class EventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<Type, Delegate> _subscriptions = new();

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : EventBase
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

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : EventBase
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

        public void Publish<TEvent>(TEvent eventData) where TEvent : EventBase
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
