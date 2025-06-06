using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
namespace LOGIYGames
{
    public class NetworkSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject voiceChatPrefab;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            }
        }

        private void OnClientConnected(ulong clientId)
        {
            SpawnVoiceChatForClient(clientId);
        }

        private void SpawnVoiceChatForClient(ulong clientId)
        {
            var voiceObj = Instantiate(voiceChatPrefab);
            voiceObj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }
    }
}