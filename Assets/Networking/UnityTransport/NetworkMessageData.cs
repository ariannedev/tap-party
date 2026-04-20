using Unity.Netcode;

/// <summary>
/// Serializable container for network messages sent via NGO RPCs.
/// </summary>
public struct NetworkMessageData : INetworkSerializable
{
    public int MessageType;
    public string Payload;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref MessageType);
        serializer.SerializeValue(ref Payload);
    }
}