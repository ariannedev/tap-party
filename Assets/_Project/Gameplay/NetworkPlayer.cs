///<summary>
/// Represents a connect player in a network session
/// </summary>
public class NetworkPlayer
{
    public string PlayerId { get; set; }
    public string PlayerName { get; set; }
    public bool IsHost { get; set; }
    public bool IsLocal { get; set; }
}