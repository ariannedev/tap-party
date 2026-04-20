using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Populates and controls a single player row in the waiting lobby list.
/// </summary>
public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _hostIcon;
    [SerializeField] private TMP_Text _localIndicator;
    [SerializeField] private Image _background;

    [Header("Colors")]
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _localPlayerColor;

    void Awake()
    {
        Debug.Assert(_playerName != null, "PlayerListItem: PlayerName reference is missing.");
    }

    public void Populate(NetworkPlayer player, bool isHost)
    {
        _playerName.text = player.PlayerName;

        if (_hostIcon != null)
            _hostIcon.gameObject.SetActive(isHost);

        if (_localIndicator != null)
            _localIndicator.gameObject.SetActive(player.IsLocal);

        if (_background != null)
            _background.color = player.IsLocal ? _localPlayerColor : _defaultColor;
    }
}