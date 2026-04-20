using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Transitions the player to the lobby scene.
/// </summary>
public class MultiplayerButton : MonoBehaviour
{
    public void OnPressed()
    {
        SceneManager.UnloadSceneAsync(1);
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }
}