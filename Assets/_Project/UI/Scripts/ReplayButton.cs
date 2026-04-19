using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Reloads the current scene to restart the game.
/// </summary>
public class ReplayButton : MonoBehaviour
{
    public void OnPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
