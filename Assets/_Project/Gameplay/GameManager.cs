// GameManager.cs
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Acts as composition root for the scene
/// Creates and owns active game sessions
/// </summary>
public class GameManager : MonoBehaviour
{
    public IGameSession Session { get; private set; }

    //accessors for ui componenets
    public ScoreCounter ScoreCounter => (Session as LocalGameSession)?.ScoreCounter;
    public GameTimer GameTimer => (Session as LocalGameSession)?.GameTimer;
    public PersonalBestTracker PersonalBestTracker => (Session as LocalGameSession)?.PersonalBestTracker;

    void Awake()
    {
        string playerName = NameGenerator.GetOrGenerateName();
        Session = new LocalGameSession("Player", 5f);
        Debug.Assert(Session != null, "GameManager: Failed to create LocalGameSession");
    }

    void Update()
    {
        (Session as LocalGameSession)?.Tick(Time.deltaTime);
    }

    public void OnTap()
    {
        Session.OnTap();
    }

    void OnDestroy()
    {
        (Session as LocalGameSession)?.Dispose();
    }
}