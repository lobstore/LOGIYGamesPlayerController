using R3;
using System;
using System.Collections.Generic;

namespace LOGIYGames
{
    [Serializable]
    public class Stat
    {
        public float BaseValue;

        private readonly List<StatModifier> _modifiers = new List<StatModifier>();
        public Subject<Unit> OnModifiersChanged = new();

        public float Value
        {
            get
            {
                float add = 0f;
                float mul = 1f;

                foreach (var modifier in _modifiers)
                {
                    switch (modifier.Type)
                    {
                        case ModifierType.Add:
                            add += modifier.Value;
                            break;

                        case ModifierType.Multiply:
                            mul += modifier.Value;
                            break;
                    }
                }

                return (BaseValue + add) * mul;
            }
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            OnModifiersChanged.OnNext(Unit.Default);
        }

        public void RemoveSource(object source)
        {
            _modifiers.RemoveAll(
                x => x.Source == source);
            OnModifiersChanged.OnNext(Unit.Default);
        }
    }

}
