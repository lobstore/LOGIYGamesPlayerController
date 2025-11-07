using LOGIYGames;
using LOGIYGames.Timers;
using UnityEngine;

public class SceneEntryPoint : MonoBehaviour
{
    [SerializeField] InputReader InputReader;
    [SerializeField] bool visibleCursor;
    [SerializeField] int targetFramerate = 30;
    void Awake()
    {

        //if (LobbyManager.Instance.JoinedLobby != null && !LobbyManager.Instance.IsHost)
        //{
        //    NetworkManager.Singleton.StartClient();
        //}
        //else
        //{
        //    NetworkManager.Singleton.StartHost();
        //}
        if (visibleCursor)
        {
            InputReader.EngageUI();
        }
        else
        {
            InputReader.DisengageUI();
        }
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFramerate;
    }

}