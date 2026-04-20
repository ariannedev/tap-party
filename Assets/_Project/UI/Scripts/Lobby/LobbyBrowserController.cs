using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the lobby browser panel. Fetches and displays available lobbies.
/// </summary>
public class LobbyBrowserController : MonoBehaviour
{
    [SerializeField] private GameObject _gameListItemPrefab;
    [SerializeField] private Transform _gameListContent;
    [SerializeField] private Button _createGameButton;
    [SerializeField] private Button _refreshButton;

    [SerializeField] private LobbyWaitingController _waitingController;

    private NGOTransport _transport;

    void Awake()
    {
        _transport = ServiceLocator.Transport;
        Debug.Assert(_transport != null, "LobbyBrowserController: Transport not found in ServiceLocator.");
        Debug.Assert(_waitingController != null, "LobbyBrowserController: WaitingController reference is missing.");
        Debug.Assert(_gameListItemPrefab != null, "LobbyBrowserController: GameListItem prefab is missing.");
        Debug.Assert(_gameListContent != null, "LobbyBrowserController: GameListContent reference is missing.");
        Debug.Assert(_createGameButton != null, "LobbyBrowserController: CreateGameButton reference is missing.");
        Debug.Assert(_refreshButton != null, "LobbyBrowserController: RefreshButton reference is missing.");
    }

    void Start()
    {
        _createGameButton.onClick.AddListener(OnCreateGamePressed);
        _refreshButton.onClick.AddListener(OnRefreshPressed);
        RefreshLobbies();
    }

    private async void RefreshLobbies()
    {
        ClearList();

        List<Lobby> lobbies = await LobbyHelper.GetAvailableLobbiesAsync();

        foreach (Lobby lobby in lobbies)
        {
            GameObject item = Instantiate(_gameListItemPrefab, _gameListContent);
            GameListItem listItem = item.GetComponent<GameListItem>();
            listItem.Populate(lobby, OnJoinPressed);
        }
    }

    private void ClearList()
    {
        foreach (Transform child in _gameListContent)
        {
            Destroy(child.gameObject);
        }
    }

    private async void OnCreateGamePressed()
    {
        _createGameButton.interactable = false;
        string playerName = NameGenerator.GetOrGenerateName();
        await _transport.CreateRoomAsync(playerName);
        // transition to waiting panel
        _waitingController.Show();
    }

    private async void OnJoinPressed(string lobbyId)
    {
        await _transport.JoinRoomAsync(lobbyId);
        // transition to waiting panel
        _waitingController.Show();
    }

    private void OnRefreshPressed()
    {
        RefreshLobbies();
    }

    void OnDestroy()
    {
        _createGameButton.onClick.RemoveAllListeners();
        _refreshButton.onClick.RemoveAllListeners();
    }
}