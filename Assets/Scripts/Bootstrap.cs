using System.Collections;
using UnityEngine;
namespace LOGIYGames
{
    public class Bootstrap : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return null;
            LevelLoader.Instance.SwitchToScene(1);
        }
    }
}