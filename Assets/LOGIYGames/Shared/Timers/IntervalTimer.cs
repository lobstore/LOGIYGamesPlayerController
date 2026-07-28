using System;
using UnityEngine;
namespace LOGIYGames.Timers
{
    [Serializable]
    public class IntervalTimer : Timer
    {
        [SerializeField] private float interval;
        [SerializeField] private float nextInterval;

        public Action OnInterval = delegate { };

        public IntervalTimer(float totalTime, float intervalSeconds,bool isGlobalTimer = true) : base(totalTime, isGlobalTimer)
        {
            interval = intervalSeconds;
            nextInterval = totalTime - interval;
        }

        public override bool IsFinished => CurrentTime.CurrentValue <= 0;

        public override void Tick()
        {
            if (IsRunning && CurrentTime.CurrentValue > 0)
            {
                CurrentTime.Value -= Time.deltaTime;

                while (CurrentTime.CurrentValue <= nextInterval && nextInterval >= 0)
                {
                    OnInterval.Invoke();
                    nextInterval -= interval;
                }
            }
            if (IsRunning && CurrentTime.CurrentValue <= 0)
            {
                CurrentTime.Value = 0;
                Stop();
            }
        }
    }
}