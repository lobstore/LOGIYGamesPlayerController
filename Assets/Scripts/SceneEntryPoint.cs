using System.Collections;
using Unity.Netcode;
using UnityEngine;
namespace LOGIYGames
{
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

    }
}