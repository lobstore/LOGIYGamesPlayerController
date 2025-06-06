using System;
using UnityEngine.Events;
namespace LOGIYGames
{
    public class PlayerLevelModel
    {
        public UnityEvent OnXPChanged { get; private set; } = new();
        public UnityEvent<int> OnLvlChanged { get; private set; } = new();

        private Func<int> RequireXpCounter;

        private int lvl = 0;
        private int currentxp = 0;
        public int Level
        {
            get { return lvl; }
            private set
            {
                if (value < 0)
                {
                    lvl = 0;
                }
                else
                {
                    lvl = value;
                }
                OnLvlChanged?.Invoke(lvl);
            }
        }
        public int RequireXP { get => RequireXpCounter.Invoke(); }
        public int CurrentXP
        {
            get { return currentxp; }
            private set
            {
                if (value < 0)
                {
                    currentxp = 0;
                }
                else
                {
                    currentxp = value;
                }
                OnXPChanged?.Invoke();
            }
        }

        public void AddXp(int amount)
        {
            CurrentXP += amount;
            CheckNewLvl();
        }
        public void Reset()
        {
            CurrentXP = 0;
            Level = 0;
        }
        private void CheckNewLvl()
        {
            if (CurrentXP >= RequireXP)
            {
                Level++;
                CurrentXP -= RequireXP;
            }
        }
        private int DefaultCounter()
        {
            return (int)(400 + (lvl * 400 * 0.2));
        }
        public PlayerLevelModel(int level, int currentXP, Func<int> requireXpCounterFunc = null)
        {
            Level = level;
            CurrentXP = currentXP;
            if (requireXpCounterFunc == null)
            {
                RequireXpCounter = DefaultCounter;
            }
            else
            {
                RequireXpCounter = requireXpCounterFunc;
            }
        }
    }
}