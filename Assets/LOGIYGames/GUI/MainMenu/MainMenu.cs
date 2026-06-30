using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LOGIYGames
{
    public class MainMenu : MonoBehaviour
    {
        public void StartGame()
        {
            LevelLoader.Instance.SwitchToScene(2);
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
