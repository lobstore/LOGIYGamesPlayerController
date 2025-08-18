using UnityEngine;

namespace LOGIYGames
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }
        protected virtual void Awake()
        {
            T[] objs = FindObjectsByType<T>(FindObjectsSortMode.None);
            if (Instance == null)
            {
                if (objs.Length > 1)
                {

                    Instance = objs[0];
                    for (int i = 1; i < objs.Length; i++)
                    {
                        Destroy(objs[i].gameObject);
                    }
                }
                else if (objs.Length == 0)
                {
                    gameObject.AddComponent<T>();
                }
            }
            else { Destroy(gameObject); }

        }
    }
}
