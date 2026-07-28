using Alchemy.Inspector;
using R3;
using System;
using UnityEngine;
namespace LOGIYGames.Timers
{
    [Serializable]
    public abstract class Timer : IDisposable
    {
        [field:SerializeField]  public bool IsGlobalTimer { get; private set; }
        [SerializeField] protected bool IsStopped;
        [SerializeField] protected float initialTime;
        [field:SerializeField] public SerializableReactiveProperty<float> CurrentTime { get; set; } = new();
        public virtual float ElapsedTime => CurrentTime.CurrentValue;
        [field: SerializeField] public bool IsRunning { get; protected set; }

        public virtual float Progress => Mathf.Clamp(CurrentTime.CurrentValue / initialTime, 0, 1);

        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value, bool isGlobalTimer = true)
        {
            IsGlobalTimer = isGlobalTimer;
            initialTime = value;
            IsRunning = false;
        }

        public void Start()
        {
            CurrentTime.Value = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                IsStopped = false;
                if (IsGlobalTimer)
                {
                    TimersManager.RegisterTimer(this);
                }
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                IsStopped = true;
                if (IsGlobalTimer)
                {
                    TimersManager.DeregisterTimer(this);
                }
                OnTimerStop.Invoke();
            }
        }


        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;
        public virtual void Reset() => CurrentTime.Value = initialTime;
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
}