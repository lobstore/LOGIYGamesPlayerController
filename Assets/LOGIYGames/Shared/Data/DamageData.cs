using LOGIYGames.Shared.Enums;
using UnityEngine;
namespace LOGIYGames.Shared.Data
{
    public struct DamageData
    {
        public int Amount;
        public GameObject Dealer;
        public Vector3 HitPoint;
        public Vector3 HitDirection;

        public DamageType Type;
    }
}
