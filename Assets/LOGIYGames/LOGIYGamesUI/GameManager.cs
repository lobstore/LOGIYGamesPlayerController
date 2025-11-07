using LOGIYGames.Timers;
using UnityEngine;
namespace LOGIYGames
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] InputReader InputReader;
        public static GameManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);

            }
            else { Destroy(gameObject); }
            InputReader.GameControlInputsEnable = true;
        }
        private void Update()
        {
            TimersManager.UpdateTimers();
        }
    }
}