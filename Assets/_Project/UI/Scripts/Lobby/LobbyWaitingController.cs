using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the waiting lobby panel. Shows connected players and host controls.
/// </summary>
public class LobbyWaitingController : MonoBehaviour
{
    [SerializeField] private GameObject _playerListItemPrefab;
    [SerializeField] private Transform _playerListContent;
    [SerializeField] private TMP_Text _playerCountText;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _leaveButton;
    [SerializeField] private GameObject _browserPanel;
    [SerializeField] private GameObject _waitingPanel;

    private NGOTransport _transport;

    void Awake()
    {
        Debug.Assert(_playerListItemPrefab != null, "LobbyWaitingController: PlayerListItem prefab is missing.");
        Debug.Assert(_playerListContent != null, "LobbyWaitingController: PlayerListContent reference is missing.");
        Debug.Assert(_playerCountText != null, "LobbyWaitingController: PlayerCountText reference is missing.");
        Debug.Assert(_startButton != null, "LobbyWaitingController: StartButton reference is missing.");
        Debug.Assert(_leaveButton != null, "LobbyWaitingController: LeaveButton reference is missing.");
        Debug.Assert(_browserPanel != null, "LobbyWaitingController: BrowserPanel reference is missing.");
        Debug.Assert(_waitingPanel != null, "LobbyWaitingController: WaitingPanel reference is missing.");
    }

    void Start()
    {
        _startButton.onClick.AddListener(OnStartPressed);
        _leaveButton.onClick.AddListener(OnLeavePressed);

    }

    public void Show()
    {
        if (_transport == null)
        {
            _transport = ServiceLocator.Transport;
            Debug.Assert(_transport != null, "LobbyWaitingController: Transport not found in ServiceLocator.");

            _transport.OnPlayerJoined += OnPlayerJoined;
            _transport.OnPlayerLeft += OnPlayerLeft;
        }
        _browserPanel.SetActive(false);
        _waitingPanel.SetActive(true);

        // only host sees start button
        _startButton.gameObject.SetActive(_transport.IsHost);

        RefreshPlayerList();
    }

    public void Hide()
    {
        _waitingPanel.SetActive(false);
        _browserPanel.SetActive(true);
    }

    private void RefreshPlayerList()
    {
        ClearList();

        foreach (NetworkPlayer player in _transport.ConnectedPlayers)
        {
            GameObject item = Instantiate(_playerListItemPrefab, _playerListContent);
            PlayerListItem listItem = item.GetComponent<PlayerListItem>();
            listItem.Populate(player, _transport.IsHost && player.IsLocal);
        }

        _playerCountText.text = $"{_transport.ConnectedPlayers.Count}/8";
    }

    private void ClearList()
    {
        foreach (Transform child in _playerListContent)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnPlayerJoined(NetworkPlayer player)
    {
        RefreshPlayerList();
    }

    private void OnPlayerLeft(NetworkPlayer player)
    {
        RefreshPlayerList();
    }

    private async void OnStartPressed()
    {
        _startButton.interactable = false;
        // will trigger game start via NetworkGameSession
    }

    private async void OnLeavePressed()
    {
        await _transport.LeaveRoomAsync();
        Hide();
    }

    void OnDestroy()
    {
        _startButton.onClick.RemoveAllListeners();
        _leaveButton.onClick.RemoveAllListeners(); ;
        _transport.OnPlayerJoined -= OnPlayerJoined;
        _transport.OnPlayerLeft -= OnPlayerLeft;
    }
}