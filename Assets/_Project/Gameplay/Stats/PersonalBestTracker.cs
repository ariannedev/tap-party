using System;
using UnityEngine;

/// <summary>
/// Tracks and persists the players personal best using PlayerPrefs
/// </summary>
public class PersonalBestTracker
{
    private const string k_PersonBestKey = "PersonalBest";

    public int PersonalBest { get; private set; }
    public bool IsNewPersonalBest { get; private set; }

    public event Action<int> OnPersonalBestChanged;

    public PersonalBestTracker()
    {
        PersonalBest = PlayerPrefs.GetInt(k_PersonBestKey, 0);
        IsNewPersonalBest = false;
    }

    public void Check(int score)
    {
        IsNewPersonalBest = score > PersonalBest;
        if (IsNewPersonalBest)
        {
            PersonalBest = score;
            PlayerPrefs.SetInt(k_PersonBestKey, PersonalBest);
            PlayerPrefs.Save();
            OnPersonalBestChanged?.Invoke(PersonalBest);
        }
    }
}
