using UnityEngine;

/// <summary>
/// Serializes and deserializes INetworkMessage to and from NetworkMessageData.
/// </summary>
public static class NetworkMessageSerializer
{
    public static NetworkMessageData Serialize(INetworkMessage message)
    {
        return message switch
        {
            PlayerJoinedMessage m => new NetworkMessageData
            {
                MessageType = (int)NetworkMessageType.PlayerJoined,
                Payload = JsonUtility.ToJson(m)
            },
            GameStartMessage m => new NetworkMessageData
            {
                MessageType = (int)NetworkMessageType.GameStart,
                Payload = JsonUtility.ToJson(m)
            },
            FinalScoreMessage m => new NetworkMessageData
            {
                MessageType = (int)NetworkMessageType.FinalScore,
                Payload = JsonUtility.ToJson(m)
            },
            GameResultsMessage m => new NetworkMessageData
            {
                MessageType = (int)NetworkMessageType.GameResults,
                Payload = JsonUtility.ToJson(m)
            },
            PlayerLeftMessage m => new NetworkMessageData
            {
                MessageType = (int)NetworkMessageType.PlayerLeft,
                Payload = JsonUtility.ToJson(m)
            },
            _ => throw new System.ArgumentException($"Unknown message type: {message.GetType()}")
        };
    }

    public static INetworkMessage Deserialize(NetworkMessageData data)
    {
        return (NetworkMessageType)data.MessageType switch
        {
            NetworkMessageType.PlayerJoined =>
                JsonUtility.FromJson<PlayerJoinedMessage>(data.Payload),
            NetworkMessageType.GameStart =>
                JsonUtility.FromJson<GameStartMessage>(data.Payload),
            NetworkMessageType.FinalScore =>
                JsonUtility.FromJson<FinalScoreMessage>(data.Payload),
            NetworkMessageType.GameResults =>
                JsonUtility.FromJson<GameResultsMessage>(data.Payload),
            NetworkMessageType.PlayerLeft =>
                JsonUtility.FromJson<PlayerLeftMessage>(data.Payload),
            _ => throw new System.ArgumentException($"Unknown message type: {data.MessageType}")
        };
    }
}