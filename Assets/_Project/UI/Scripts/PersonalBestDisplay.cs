using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays the players personal best score
/// Updates when a new personal best is set
/// </summary>
public class PersonalBestDisplay : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private TMP_Text _personalBestText;

    void Awake()
    {
        Debug.Assert(_gameManager != null, "PersonalBestDisplay: GameManager reference is missing.");
        Debug.Assert(_personalBestText != null, "PersonalBestDisplay: Personal best text reference is missing.");
    }
    void Start()
    {
        if (_gameManager == null) return;
        _gameManager.PersonalBestTracker.OnPersonalBestChanged += OnPersonalBestChanged;
        UpdateDisplay(_gameManager.PersonalBestTracker.PersonalBest);
    }

    void OnPersonalBestChanged(int newPersonalBest)
    {
        UpdateDisplay(newPersonalBest);
    }

    void UpdateDisplay(int personalBest)
    {
        if (_personalBestText == null) return;
        _personalBestText.text = $"Personal Best: {personalBest}";
    }

    void OnDestroy()
    {
        if (_gameManager == null) return;
        _gameManager.PersonalBestTracker.OnPersonalBestChanged -= OnPersonalBestChanged;
    }
}
