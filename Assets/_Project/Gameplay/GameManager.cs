// GameManager.cs
using UnityEngine;

/// <summary>
/// Holds and provides access to game logic objects, acting as the composition root for the scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    public ScoreCounter ScoreCounter { get; private set; }

    void Awake()
    {
        ScoreCounter = new ScoreCounter();
        Debug.Assert(ScoreCounter != null, "GameManager: Failed to create ScoreCounter.");
        if (ScoreCounter == null) return;
    }
}