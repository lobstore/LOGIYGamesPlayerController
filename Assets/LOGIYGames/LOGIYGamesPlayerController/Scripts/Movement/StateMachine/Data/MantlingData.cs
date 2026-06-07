using System;
using UnityEngine;

namespace LOGIYGames
{
    [Serializable]
        public struct MantlingData
        {
            public float duration;
            public float checkDistance;
            public LayerMask mantlingLayers;
        }
}
