
using UnityEngine;


/// <summary>
/// Generates a random player name that persists across sessions
/// </summary>
public static class NameGenerator
{
    private const string k_PlayerNameKey = "PlayerName";

    private static readonly string[] Adjectives =
    {
        "Bouncy", "Speedy", "Sneaky", "Grumpy", "Fluffy",
        "Spicy", "Wobbly", "Zippy", "Cranky", "Sleepy",
        "Jumpy", "Dizzy", "Fancy", "Cheeky", "Witty"
    };

    private static readonly string[] Nouns =
    {
        "Snail", "Pickle", "Badger", "Waffle", "Noodle",
        "Penguin", "Muffin", "Llama", "Biscuit", "Goblin",
        "Turnip", "Panda", "Nugget", "Narwhal", "Platypus"
    };

    public static string GetOrGenerateName()
    {
        if (PlayerPrefs.HasKey(k_PlayerNameKey))
            return PlayerPrefs.GetString(k_PlayerNameKey);

        string name = GenerateName();
        SaveName(name);
        return name;
    }

    private static string GenerateName()
    {
        string adjective = Adjectives[Random.Range(0, Adjectives.Length)];
        string noun = Nouns[Random.Range(0, Nouns.Length)];
        return $"{adjective}{noun}";
    }

    private static void SaveName(string name)
    {
        PlayerPrefs.SetString(k_PlayerNameKey, name);
        PlayerPrefs.Save();
    }
}
