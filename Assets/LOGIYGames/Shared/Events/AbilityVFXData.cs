using System;
using UnityEngine;

namespace LOGIYGames.Shared.Character.Events
{
    [Serializable]
    public struct AbilityVFXData
    {
        public GameObject vfxPrefab;
        public Vector3 vfxPositionOffset;
        public Quaternion vfxRotationOffset;
        public Vector3 vfxScale;
    }

}