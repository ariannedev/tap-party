// ScoreDisplay.cs
using TMPro;
using UnityEngine;

/// <summary>
/// Receives changes to the score and updates the display.
/// </summary>
public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private TMP_Text _scoreText;

    void Awake()
    {
        Debug.Assert(_gameManager != null, "ScoreDisplay: GameManager reference is missing. Please assign it in the Inspector.");
        Debug.Assert(_scoreText != null, "ScoreDisplay: Score text reference is missing. Please assign it in the Inspector.");
    }

    void Start()
    {
        if (_gameManager == null) return;
        _gameManager.ScoreCounter.OnScoreChanged += OnScoreChanged;
    }

    void OnScoreChanged(int newScore)
    {
        if (_scoreText == null) return;
        _scoreText.text = newScore.ToString();
    }

    void OnDestroy()
    {
        _gameManager.ScoreCounter.OnScoreChanged -= OnScoreChanged;
    }
}