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
            return _stats[stat];
        }

        public void SetBase(StatType stat, float value)
        {
            _stats[stat].BaseValue = value;
        }

        public void AddModifier(StatType stat, StatModifier modifier)
        {
            _stats[stat].AddModifier(modifier);
        }

    }

}
