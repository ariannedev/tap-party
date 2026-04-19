///<summary>
/// Represents the result of a single player at the end of a game session
/// </summary>
public struct PlayerResult
{
    public string PlayerName;
    public int Score;

    public PlayerResult(string playerName, int score)
    {
        PlayerName = playerName;
        Score = score;
    }
}