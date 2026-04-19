using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

/// <summary>
/// Initialises Unity Gaming Services and signs in anonymously on Startup
/// </summary>
public class ServicesInitialiser : MonoBehaviour
{
    public bool IsInitialised { get; private set; }

    async void Awake()
    {
        await InitialiseAsync();
    }

    private async Task InitialiseAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            IsInitialised = true;
            Debug.Log($"ServicesInitialiser: Signed in as {AuthenticationService.Instance.PlayerId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ServicesInitialiser: Failed to initialise - {e.Message}");
        }
    }
}
