using System.Collections;
using UnityEngine;

namespace LOGIYGames
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] int sceneToLoad;
        [SerializeField] float waitSeconds;
        IEnumerator Start()
        {
            yield return new WaitForSeconds(waitSeconds);
            LevelLoader.Instance.SwitchToScene(sceneToLoad);
        }

    }
}
