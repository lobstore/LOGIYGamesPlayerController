using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public class StatsController
    {
        [SerializeField] private Dictionary<StatType, Stat> _stats;
        public IReadOnlyDictionary<StatType, Stat> Stats => _stats;
        public Stat GetStat(StatType stat)
        {
            if (!_stats.ContainsKey(stat))
            {
                throw new ArgumentException($"Stat of type {stat} does not exist.");
            }
            return _stats[stat];
        }

        public void SetBase(StatType stat, int value)
        {
            if (!_stats.ContainsKey(stat))
            {
                throw new ArgumentException($"Stat of type {stat} does not exist.");
            }
            _stats[stat].BaseValue = value;
        }

        public void AddModifier(StatType stat, StatModifier modifier)
        {
            if (!_stats.ContainsKey(stat))
            {
                throw new ArgumentException($"Stat of type {stat} does not exist.");
            }
            _stats[stat].AddModifier(modifier);
        }

    }

}
