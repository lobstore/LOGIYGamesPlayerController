using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames.Timers
{
    public static class TimersManager
    {
        static readonly List<Timer> timers = new();

        public static void RegisterTimer(Timer timer) => timers.Add(timer);
        public static void DeregisterTimer(Timer timer) => timers.Remove(timer);

        public static void UpdateTimers()
        {
            foreach (var timer in new List<Timer>(timers) )
            {
                timer.Tick();
            }
        }
        public static void Clear() => timers.Clear();
    }
    [Serializable]
    public abstract class Timer : IDisposable
    {
        protected bool IsStopped;
        protected float initialTime;
        public float CurrentTime { get; set; }
        public bool IsRunning { get; protected set; }

        public float Progress => Mathf.Clamp(CurrentTime / initialTime, 0, 1);

        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value)
        {
            initialTime = value;
            IsRunning = false;
        }

        public void Start()
        {
            CurrentTime = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                IsStopped = false;
                TimersManager.RegisterTimer(this);
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                IsStopped = true;
                TimersManager.DeregisterTimer(this);
                OnTimerStop.Invoke();
            }
        }


        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;
        public virtual void Reset() => CurrentTime = initialTime;
        public virtual void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }

        public abstract void Tick();
        public abstract bool IsFinished { get; }

        bool disposed;

        ~Timer()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                TimersManager.DeregisterTimer(this);
            }
            disposed = true;
        }

    }
    [Serializable]
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value) : base(value) { }
        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0)
            {
                CurrentTime -= Time.deltaTime;
            }

            if (IsRunning && CurrentTime <= 0)
            {
                Stop();
            }
        }

        public override bool IsFinished => CurrentTime <= 0;

    }
    [Serializable]
    public class StopwatchTimer : Timer
    {
        public StopwatchTimer() : base(0) { }

        public override void Tick()
        {
            if (IsRunning)
            {
                CurrentTime += Time.deltaTime;
            }
        }
        public override bool IsFinished => IsStopped;
    }
    [Serializable]
    public class IntervalTimer : Timer
    {
        readonly float interval;
        float nextInterval;

        public Action OnInterval = delegate { };

        public IntervalTimer(float totalTime, float intervalSeconds) : base(totalTime)
        {
            interval = intervalSeconds;
            nextInterval = totalTime - interval;
        }

        public override bool IsFinished => CurrentTime<=0;

        public override void Tick()
        {
            if (IsRunning&&CurrentTime>0)
            {
                CurrentTime -= Time.deltaTime;

                while (CurrentTime <= nextInterval && nextInterval >= 0)
                {
                    OnInterval.Invoke();
                    nextInterval-=interval;
                }
            }
            if (IsRunning&&CurrentTime<=0)
            {
                CurrentTime = 0;
                Stop();
            }
        }
    }
}