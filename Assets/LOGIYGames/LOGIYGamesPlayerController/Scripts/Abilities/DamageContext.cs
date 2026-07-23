using LOGIYGames.CharacterCore;
using LOGIYGames.Shared.Data;
using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
    public sealed class DamageContext
    {
        public GameObject Source;
        public GameObject Target;

        public DamageData Damage;

        public bool IsCritical;

        public bool Cancelled;
    }

}
