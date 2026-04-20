using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles NGO RPC messaging between host and clients.
/// Attached to a NetworkObject in the scene.
/// </summary>
public class NGOMessenger : NetworkBehaviour
{
    public event Action<INetworkMessage> OnMessageReceived;

    // Host → All Clients
    [ClientRpc]
    public void SendToAllClientRpc(NetworkMessageData data)
    {
        INetworkMessage message = NetworkMessageSerializer.Deserialize(data);
        OnMessageReceived?.Invoke(message);
    }

    // Client → Host
    [ServerRpc(RequireOwnership = false)]
    public void SendToHostServerRpc(NetworkMessageData data)
    {
        INetworkMessage message = NetworkMessageSerializer.Deserialize(data);
        OnMessageReceived?.Invoke(message);
    }
}