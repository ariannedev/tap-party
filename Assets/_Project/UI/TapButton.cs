// TapButton.cs
using UnityEngine;

/// <summary>
/// Detects player input and forwards it to the game logic.
/// </summary>
public class TapButton : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    void Awake()
    {
        Debug.Assert(_gameManager != null, "TapButton: GameManager reference is missing. Please assign it in the Inspector.");
    }

    public void OnTapped()
    {
        if (_gameManager == null) return;
        _gameManager.ScoreCounter.Increment();
    }
}