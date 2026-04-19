using System.Collections;
using UnityEngine;

/// <summary>
/// Coordinates the game over sequence.
/// </summary>
public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private ScreenShake _screenShake;
    [SerializeField] private GameObject _replayButton;

    void Awake()
    {
        Debug.Assert(_gameManager != null, "GameOverDisplay: GameManager reference is missing. Please assign it in the Inspector.");
        Debug.Assert(_screenShake != null, "GameOverDisplay: ScreenShake reference is missing. Please assign it in the Inspector.");
        Debug.Assert(_screenShake != null, "GameOverDisplay: Replay Button reference is missing. Please assign it in the Inspector.");
        if (_gameManager == null) return;
        if (_screenShake == null) return;
    }

    void Start()
    {

        if (_gameManager == null) return;
        _gameManager.GameTimer.OnExpired += OnExpired;
    }

    void OnExpired()
    {
        if (_screenShake == null) return;
        StartCoroutine(PlayGameOverSequence());
    }

    private IEnumerator PlayGameOverSequence()
    {
        float duration = _screenShake.Shake(0.3f, 20f);
        yield return new WaitForSeconds(duration);
        _replayButton.SetActive(true);
    }

    void OnDestroy()
    {
        if (_gameManager == null) return;
        _gameManager.GameTimer.OnExpired -= OnExpired;
    }
}
