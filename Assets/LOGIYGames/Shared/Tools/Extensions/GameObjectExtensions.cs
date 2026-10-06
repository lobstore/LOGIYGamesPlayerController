using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace LOGIYGames.Shared.Extensions {
    public static class GameObjectExtensions {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go.TryGetComponent<T>(out var t))
            {
                return t;
            }
            return go.AddComponent<T>();
        }
        public static bool IsLayerInMask(this GameObject go, LayerMask mask)
        {
            return (mask.value & (1 << go.layer)) != 0;
        }
    }
}