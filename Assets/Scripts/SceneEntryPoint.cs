using System.Collections;
using Unity.Netcode;
using UnityEngine;
namespace LOGIYGames
{
    public class SceneEntryPoint : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (LobbyManager.Instance.JoinedLobby != null && !LobbyManager.Instance.IsHost)
            {
                NetworkManager.Singleton.StartClient();
            }
            else
            {
                NetworkManager.Singleton.StartHost();
            }
        }

    }
}