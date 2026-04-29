using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using UnityEngine;

/// <summary>
/// INetworkTransport implementation using Unity NGO, Relay, and Lobby.
/// </summary>
public class NGOTransport : MonoBehaviour, INetworkTransport
{
    // INetworkTransport
    public bool IsHost { get; private set; }
    public string LocalPlayerId =>
        Unity.Services.Authentication.AuthenticationService.Instance.PlayerId;
    public IReadOnlyList<NetworkPlayer> ConnectedPlayers => _connectedPlayers;

    [SerializeField] private NGOMessenger _messengerPrefab;

    public event Action<NetworkPlayer> OnPlayerJoined;
    public event Action<NetworkPlayer> OnPlayerLeft;
    public event Action<INetworkMessage> OnMessageReceived;

    private NGOMessenger _messenger;
    private readonly List<NetworkPlayer> _connectedPlayers = new List<NetworkPlayer>();
    private Lobby _currentLobby;
    private string _playerName;

    public async Task CreateRoomAsync(string playerName)
    {
        _playerName = playerName;
        IsHost = true;

        // Step 1 — create relay allocation
        var (joinCode, allocation) = await RelayHelper.CreateAllocationAsync();

        // Step 2 — configure NGO to use relay
        ConfigureHostRelay(allocation);

        // Step 3 — create lobby storing relay join code
        _currentLobby = await LobbyHelper.CreateLobbyAsync(playerName, joinCode);

        // Step 4 — start NGO host
        NetworkManager.Singleton.StartHost();

        // Step 5 — register NGO callbacks
        RegisterNGOCallbacks();

        // Step 6 — spawn messenger and subscribe
        GameObject messengerObj = Instantiate(_messengerPrefab.gameObject);
        NetworkObject networkObject = messengerObj.GetComponent<NetworkObject>();
        networkObject.Spawn();
        _messenger = messengerObj.GetComponent<NGOMessenger>();
        _messenger.OnMessageReceived += HandleMessageReceived;

        // Step 7 — add ourselves to connected players
        AddLocalPlayer(playerName, true);

        Debug.Log($"NGOTransport: Room created, lobby {_currentLobby.Id}");
    }

    public async Task JoinRoomAsync(string roomId)
    {
        IsHost = false;
        _playerName = PlayerProfile.PlayerName;

        // Step 1 — get relay join code from lobby
        Lobby lobby = await LobbyHelper.JoinLobbyAsync(roomId);
        _currentLobby = lobby;
        string relayJoinCode = LobbyHelper.GetRelayJoinCode(lobby);

        // Step 2 — join relay allocation and configure NGO
        JoinAllocation allocation = await RelayHelper.JoinAllocationAsync(relayJoinCode);
        ConfigureClientRelay(allocation);
        NetworkManager.Singleton.StartClient();
        RegisterNGOCallbacks();

        // Step 3 — wait for NGO to spawn the messenger
        _messenger = null;
        while (_messenger == null || !_messenger.IsSpawned)
        {
            await System.Threading.Tasks.Task.Delay(100);
            _messenger = GameObject.FindObjectOfType<NGOMessenger>();
        }
        _messenger.OnMessageReceived += HandleMessageReceived;
        Debug.Log($"JoinRoomAsync: messenger ready");

        // Step 4 — announce ourselves to the host
        await SendToHostAsync(new PlayerJoinedMessage
        {
            PlayerId = LocalPlayerId,
            PlayerName = _playerName
        });

        Debug.Log($"NGOTransport: Joined room {roomId}");
    }

    public async Task LeaveRoomAsync()
    {
        if (_currentLobby == null) return;

        try
        {
            bool isLastPlayer = _currentLobby.Players.Count <= 1;

            if (isLastPlayer)
            {
                await LobbyHelper.DeleteLobbyAsync(_currentLobby.Id);
            }
            else if (IsHost)
            {
                string newHostId = GetNextHostId();
                await LobbyHelper.MigrateHostAsync(_currentLobby.Id, newHostId);
                await LobbyHelper.RemovePlayerAsync(_currentLobby.Id, LocalPlayerId);
            }
            else
            {
                await LobbyHelper.RemovePlayerAsync(_currentLobby.Id, LocalPlayerId);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"NGOTransport: Failed to leave room cleanly - {e.Message}");
        }
        finally
        {
            NetworkManager.Singleton.Shutdown();
            _currentLobby = null;
        }
    }

    public async Task SendToAllAsync(INetworkMessage message)
    {
        NetworkMessageData data = NetworkMessageSerializer.Serialize(message);
        _messenger.SendToAllClientRpc(data);
        await Task.CompletedTask;
    }

    public async Task SendToHostAsync(INetworkMessage message)
    {
        NetworkMessageData data = NetworkMessageSerializer.Serialize(message);
        _messenger.SendToHostServerRpc(data);
        await Task.CompletedTask;
    }

    private void ConfigureHostRelay(Allocation allocation)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);
    }

    private void ConfigureClientRelay(JoinAllocation allocation)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);
    }

    private void RegisterNGOCallbacks()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"NGOTransport: Client connected {clientId}");
        if (!IsHost) return;

        // broadcast host info to all clients
        _ = SendToAllAsync(new PlayerJoinedMessage
        {
            PlayerId = LocalPlayerId,
            PlayerName = _playerName
        });
    }

    private void OnClientDisconnected(ulong clientId)
    {
        NetworkPlayer player = _connectedPlayers.Find(p => p.PlayerId == clientId.ToString());
        if (player == null) return;
        _connectedPlayers.Remove(player);
        OnPlayerLeft?.Invoke(player);
    }

    private void AddLocalPlayer(string playerName, bool isHost)
    {
        _connectedPlayers.Add(new NetworkPlayer
        {
            PlayerId = LocalPlayerId,
            PlayerName = playerName,
            IsHost = isHost,
            IsLocal = true
        });
    }

    private string GetNextHostId()
    {
        foreach (Player player in _currentLobby.Players)
        {
            if (player.Id != LocalPlayerId)
                return player.Id;
        }
        return null;
    }

    private void HandleMessageReceived(INetworkMessage message)
    {
        if (message is PlayerJoinedMessage joinedMessage)
        {
            if (_connectedPlayers.Exists(p => p.PlayerId == joinedMessage.PlayerId)) return;

            NetworkPlayer player = new NetworkPlayer
            {
                PlayerId = joinedMessage.PlayerId,
                PlayerName = joinedMessage.PlayerName,
                IsHost = joinedMessage.PlayerId == LocalPlayerId && IsHost,
                IsLocal = joinedMessage.PlayerId == LocalPlayerId
            };

            _connectedPlayers.Add(player);
            OnPlayerJoined?.Invoke(player);
        }
        else if (message is PlayerLeftMessage leftMessage)
        {
            NetworkPlayer player = _connectedPlayers.Find(p => p.PlayerId == leftMessage.PlayerId);
            if (player == null) return;
            _connectedPlayers.Remove(player);
            OnPlayerLeft?.Invoke(player);
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}