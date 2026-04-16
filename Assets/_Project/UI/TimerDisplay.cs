using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Receives timer ticks and updates the countdown display
/// </summary>
public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private TMP_Text _timerText;

    void Awake()
    {
        Debug.Assert(_gameManager != null, "TimerDisplay: GameManager reference is missing. Please assign it in the Inspector.");
        Debug.Assert(_timerText != null, "TimerDisplay: Timer Text reference is missing. Please assign it in the Inspector. ");

        if (_gameManager == null) return;
        if (_timerText == null) return;
    }


    void Start()
    {
        if (_gameManager == null) return;
        _gameManager.GameTimer.OnTick += OnTick;
        _gameManager.GameTimer.OnExpired += OnExpired;

        UpdateDisplay(_gameManager.GameTimer.TimeRemaining);
    }

    void OnTick(float timeRemaining)
    {
        UpdateDisplay(timeRemaining);
    }

    void OnExpired()
    {
        UpdateDisplay(0f);
    }

    void UpdateDisplay(float timeRemaining)
    {
        if (_timerText == null) return;
        int seconds = Mathf.FloorToInt(timeRemaining);
        int centiSeconds = Mathf.FloorToInt((timeRemaining - seconds) * 100f);
        _timerText.text = string.Format("{0}:{1:00}", seconds, centiSeconds);
    }

    void OnDestroy()
    {
        if (_gameManager == null) return;
        _gameManager.GameTimer.OnTick -= OnTick;
        _gameManager.GameTimer.OnExpired -= OnExpired;
    }
}
