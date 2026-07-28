using System.Collections.Generic;
namespace LOGIYGames.Timers
{
    public class TimersManager : PersistentSingleton<TimersManager>
    {
        static readonly List<Timer> timers = new();

        public static void RegisterTimer(Timer timer) => timers.Add(timer);
        public static void DeregisterTimer(Timer timer)
        {
            if (timers.Contains(timer))
            timers.Remove(timer);
        }

        void Update()
        {
            foreach (var timer in new List<Timer>(timers))
            {
                timer.Tick();
            }
        }
        public static void Clear() => timers.Clear();
    }
}