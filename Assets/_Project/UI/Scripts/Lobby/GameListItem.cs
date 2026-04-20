using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Populates and controls a single lobby row in the browser list.
/// </summary>
public class GameListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _hostName;
    [SerializeField] private TMP_Text _playerCount;
    [SerializeField] private Button _joinButton;
    [SerializeField] private Image _background;

    private string _lobbyId;
    private Action<string> _onJoinPressed;

    void Awake()
    {
        Debug.Assert(_hostName != null, "GameListItem: HostName reference is missing.");
        Debug.Assert(_playerCount != null, "GameListItem: PlayerCount reference is missing.");
        Debug.Assert(_joinButton != null, "GameListItem: JoinButton reference is missing.");
    }

    public void Populate(Lobby lobby, Action<string> onJoinPressed)
    {
        _lobbyId = lobby.Id;
        _onJoinPressed = onJoinPressed;

        _hostName.text = lobby.Name;
        _playerCount.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";

        bool isFull = lobby.Players.Count >= lobby.MaxPlayers;
        _joinButton.interactable = !isFull;

        if (isFull && _background != null)
            _background.color = new Color(0.3f, 0.3f, 0.3f);

        _joinButton.onClick.AddListener(() => _onJoinPressed?.Invoke(_lobbyId));
    }

    void OnDestroy()
    {
        _joinButton.onClick.RemoveAllListeners();
    }
}