using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace LOGIYGames
{
    public class RelayManager : MonoBehaviour
    {
        public static RelayManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public async Task JoinRelay(string joinCode, string connectonType)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                RelayServerData relayServerData = new RelayServerData(joinAllocation, connectonType);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            }
            catch (RelayServiceException e)
            {

                Debug.Log(e);
            }

        }
        public async Task<string> CreateRelay(int maxPlayers)
        {
            try
            {
                if (maxPlayers <= 1)
                {
                    maxPlayers = 1;
                }
                else
                {
                    maxPlayers -= 1;
                }
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);


                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);


                return joinCode;
            }
            catch (RelayServiceException e)
            {

                Debug.Log(e);
                return default;
            }
        }
    }
}