using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

/// <summary>
/// Handles creation, discovery, and joining unity lobbies
/// </summary>
public static class LobbyHelper
{
    private const string k_RelayJoinCodeKey = "RelayJoinCode";
    private const string k_HostNameKey = "HostName";

    public static async Task<Lobby> CreateLobbyAsync(string hostName, string relayJoinCode)
    {
        try
        {
            Debug.Log($"LobbyHelper: Creating lobby with name {hostName}");
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    {
                        k_RelayJoinCodeKey, new DataObject(
                            visibility: DataObject.VisibilityOptions.Public,
                            value: relayJoinCode)
                    },
                    {
                        k_HostNameKey, new DataObject(
                            visibility: DataObject.VisibilityOptions.Public,
                            value: hostName)
                    }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(
                hostName,
                8,
                options);

            Debug.Log($"LobbyHelper: Lobby created with ID {lobby.Id}");
            return lobby;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LobbyHelper: Failed to create lobby - {e.Message}");
            throw;
        }
    }

    public static async Task<List<Lobby>> GetAvailableLobbiesAsync()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Order = new List<QueryOrder>
                {
                    new QueryOrder(
                        asc: false,
                        field: QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);
            return response.Results;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LobbyHelper: Failed to query lobbies - {e.Message}");
            throw;
        }
    }

    public static async Task<Lobby> JoinLobbyAsync(string lobbyId)
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            Debug.Log($"LobbyHelper: Joined lobby {lobbyId}");
            return lobby;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LobbyHelper: Failed to join lobby - {e.Message}");
            throw;
        }
    }

    public static async Task DeleteLobbyAsync(string lobbyId)
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            Debug.Log($"LobbyHelper: Deleted lobby {lobbyId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LobbyHelper: Failed to delete lobby - {e.Message}");
            throw;
        }
    }

    public static string GetRelayJoinCode(Lobby lobby)
    {
        return lobby.Data[k_RelayJoinCodeKey].Value;
    }

    public static async Task<Lobby> MigrateHostAsync(string lobbyId, string newHostPlayerId)
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(lobbyId, new UpdateLobbyOptions
            {
                HostId = newHostPlayerId
            });

            Debug.Log($"LobbyHelper: Host migrated to {newHostPlayerId}");
            return lobby;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LobbyHelper: Failed to migrate host - {e.Message}");
            throw;
        }
    }

    public static async Task RemovePlayerAsync(string lobbyId, string playerId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
            Debug.Log($"LobbyHelper: Removed player {playerId} from lobby {lobbyId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LobbyHelper: Failed to remove player - {e.Message}");
            throw;
        }
    }
}