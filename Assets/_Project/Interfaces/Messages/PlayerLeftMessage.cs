using UnityEngine;

public class PlayerLeftMessage : INetworkMessage
{
    public string PlayerId { get; set; }
    public string PlayerName { get; set; }
}
