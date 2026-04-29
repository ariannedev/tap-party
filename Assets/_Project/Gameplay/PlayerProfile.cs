/// <summary>
/// Holds persistent player identity data for the current session
/// </summary>
public static class PlayerProfile
{
    public static string PlayerName { get; private set; }

    public static void Initialise()
    {
        PlayerName = NameGenerator.GetOrGenerateName();
    }
}