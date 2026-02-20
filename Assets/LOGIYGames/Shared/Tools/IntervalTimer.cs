using System;
using UnityEngine;
namespace LOGIYGames.Timers
{
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