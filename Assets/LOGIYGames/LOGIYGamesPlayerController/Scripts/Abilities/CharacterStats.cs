using System;
using System.Collections.Generic;

namespace LOGIYGames
{
    [Serializable]
    public class CharacterStats
    {
        private readonly Dictionary<StatType, Stat> _stats = new();

        public CharacterStats()
        {
            foreach (StatType stat in Enum.GetValues(typeof(StatType)))
            {
                _stats.Add(stat, new Stat());
            }
        }

        public float Get(StatType stat)
        {
            return _stats[stat].Value;
        }

        public void SetBase(
            StatType stat,
            float value)
        {
            _stats[stat].BaseValue = value;
        }

        public void AddModifier(
            StatType stat,
            StatModifier modifier)
        {
            _stats[stat].AddModifier(modifier);
        }

        public void RemoveSource(object source)
        {
            foreach (var stat in _stats.Values)
            {
                stat.RemoveSource(source);
            }
        }
    }

}
