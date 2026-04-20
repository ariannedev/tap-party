using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps bootstrap scene objects alive across scene loads
/// Loads first scene
/// </summary>
public class BootstrapManager : MonoBehaviour
{
    [SerializeField] private ServicesInitialiser _servicesInitialiser;
    [SerializeField] private NGOTransport _transport;

    async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ServiceLocator.Register(_servicesInitialiser);
        ServiceLocator.Register(_transport);
        await WaitForServices();
        LoadGameScene();
    }

    private async System.Threading.Tasks.Task WaitForServices()
    {
        while (!_servicesInitialiser.IsInitialised)
        {
            await System.Threading.Tasks.Task.Yield();
        }
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }
}