/// <summary>
/// Identifies the type of a network message for deserialization.
/// </summary>
public enum NetworkMessageType
{
    PlayerJoined = 0,
    GameStart = 1,
    FinalScore = 2,
    GameResults = 3,
    PlayerLeft = 4
}