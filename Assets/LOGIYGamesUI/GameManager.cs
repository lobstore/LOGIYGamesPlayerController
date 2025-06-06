using UnityEngine;
namespace LOGIYGames
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance { get; private set; }

        [SerializeField] string productPath;
        [SerializeField] string settingsPath;
        public StoreProductRepository StoreProductRepository { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);

            }
            else { Destroy(gameObject); }
        }
    }
}