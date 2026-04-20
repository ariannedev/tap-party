using System;
using System.Collections.Generic;
using System.Threading.Tasks;

///<summary>
/// Defines the contract for network transport implementations
/// Handles all communication between players
/// </summary>
public interface INetworkTransport
{
    //state
    bool IsHost { get; }
    string LocalPlayerId { get; }
    IReadOnlyList<NetworkPlayer> ConnectedPlayers { get; }

    //events
    event Action<NetworkPlayer> OnPlayerJoined;
    event Action<NetworkPlayer> OnPlayerLeft;
    event Action<INetworkMessage> OnMessageReceived;

    //room management
    Task CreateRoomAsync();
    Task JoinRoomAsync(string RoomId);
    Task LeaveRoomAsync();

    //messaging
    Task SendToAllAsync(INetworkMessage message);
    Task SendToHostAsync(INetworkMessage message);
}