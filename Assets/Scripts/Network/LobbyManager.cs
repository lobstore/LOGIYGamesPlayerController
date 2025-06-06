using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
namespace LOGIYGames
{
    enum EncryptionType
    {
        DTLS,
        WSS
    }

    public class LobbyManager : MonoBehaviour
    {
        private const string KEY_START_GAME = "START";
        public static LobbyManager Instance { get; private set; }
        [SerializeField] EncryptionType encryptionType;
        public Lobby JoinedLobby { get; private set; }

        public string ConnectonType => encryptionType == EncryptionType.DTLS ? DTLS_CONST : WSS_CONST;
        private const string DTLS_CONST = "dtls";
        private const string WSS_CONST = "wss";

        public bool IsHost { get; private set; }
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


        private void Start()
        {
            InitializeUnityAuth();
        }
        private async void InitializeUnityAuth()
        {
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    await UnityServices.InitializeAsync();
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            catch (AuthenticationException e)
            {

                Debug.Log(e);
            }

        }
        public async Task JoinLobby(LobbyModel lobby)
        {
            try
            {
                if (lobby != null && !string.IsNullOrWhiteSpace(lobby.LobbyId))
                {
                    JoinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.LobbyId); ;
                    await RelayManager.Instance.JoinRelay(lobby.RelayCode, ConnectonType);
                }
                else
                {
                    Debug.Log("Что то пошло не так");
                }
            }
            catch (LobbyServiceException e)
            {

                Debug.Log(e);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        }
        public async void QuickJoinLobby()
        {
            try
            {

                JoinedLobby = await Lobbies.Instance.QuickJoinLobbyAsync();

                if (JoinedLobby.Data.TryGetValue(KEY_START_GAME, out DataObject relayData))
                {
                    string relayCode = relayData.Value;
                    await RelayManager.Instance.JoinRelay(relayCode, ConnectonType);
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

        }
        public async Task<List<LobbyModel>> GetLobbiesListAsync()
        {
            try
            {
                QueryLobbiesOptions queryOptions = new QueryLobbiesOptions
                {
                    Count = 25,
                };

                QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);

                List<LobbyModel> lobbyModels = new List<LobbyModel>();
                foreach (Lobby lobby in lobbies.Results)
                {
                    lobbyModels.Add(ConvertLobbyToModel(lobby));
                }

                return lobbyModels;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Ошибка при получении списка лобби: {e.Message}");
                return null;
            }
        }
        private LobbyModel ConvertLobbyToModel(Lobby lobby)
        {

            string relayCode = lobby.Data[KEY_START_GAME].Value;
            var lobbyModel = new LobbyModel
            {
                Name = lobby.Name,
                LobbyId = lobby.Id,
                LobbyCode = lobby.LobbyCode,
                RelayCode = relayCode,
                MaxPlayers = lobby.MaxPlayers,
                CurruntPlayersCount = lobby.Players.Count,
                IsPrivate = lobby.IsPrivate
            };
            return lobbyModel;
        }


        public async Task CreateLobby(LobbyModel lobbyModel)
        {
            try
            {
                string relayCode = await RelayManager.Instance.CreateRelay(lobbyModel.MaxPlayers);
                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    IsPrivate = lobbyModel.IsPrivate,
                    Data = new Dictionary<string, DataObject>
            {
                {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Public, relayCode) }
            }

                };

                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyModel.Name, lobbyModel.MaxPlayers, options);

                JoinedLobby = lobby;
                KeepLobbyAlive();
                IsHost = true;
            }
            catch (LobbyServiceException e)
            {

                Debug.Log(e);
            }

        }
        async void KeepLobbyAlive()
        {
            try
            {
                while (JoinedLobby != null && IsLobbyHost())
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(JoinedLobby.Id);
                    await Task.Delay(15000);
                }
            }
            catch (LobbyServiceException e)
            {

                Debug.Log(e);
            }

        }
        private bool IsLobbyHost()
        {
            return AuthenticationService.Instance.PlayerId == JoinedLobby.HostId;
        }



        private void OnApplicationQuit()
        {
            _ = LeaveLobbyOnQuit().ContinueWith(t =>
            {
                if (t.IsFaulted)
                    Debug.LogError($"Disconnect failed: {t.Exception}");
            });
        }
        public async Task LeaveLobby()
        {

            try
            {
                if (JoinedLobby != null)
                {
                    if (IsLobbyHost())
                    {
                        await LobbyService.Instance.DeleteLobbyAsync(JoinedLobby.Id);
                        JoinedLobby = null;

                    }
                    else
                    {
                        await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);
                        JoinedLobby = null;
                    }

                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }

        }
        public async Task LeaveLobbyOnQuit()
        {

            try
            {
                if (JoinedLobby != null)
                {
                    if (IsLobbyHost())
                    {
                        await LobbyService.Instance.DeleteLobbyAsync(JoinedLobby.Id);
                        JoinedLobby = null;

                    }
                    else
                    {
                        await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);
                        JoinedLobby = null;
                    }

                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }

        }
    }
}