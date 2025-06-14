using UnityEngine;
namespace LOGIYGames
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public GameInputs InputActions { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                if (InputActions == null)
                {
                    InputActions = new GameInputs();

                }
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }
}