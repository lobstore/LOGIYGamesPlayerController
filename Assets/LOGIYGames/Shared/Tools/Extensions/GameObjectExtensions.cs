using UnityEngine;

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
    }
}