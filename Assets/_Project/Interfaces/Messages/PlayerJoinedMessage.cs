
public class PlayerJoinedMessage : INetworkMessage
{
    public string PlayerId { get; set; }
    public string PlayerName { get; set; }
}
