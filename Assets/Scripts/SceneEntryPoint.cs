using LOGIYGames;
using LOGIYGames.Timers;
using UnityEngine;

public class SceneEntryPoint : MonoBehaviour
{
    [SerializeField] InputReader InputReader;
    [SerializeField] bool visibleCursor;
    void Start()
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
    }
    private void Update()
    {
        TimersManager.UpdateTimers();
    }
}