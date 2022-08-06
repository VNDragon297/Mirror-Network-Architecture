using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button ExitButton;
    [SerializeField] private List<PlayerSlotUI> playerSlots;

    private int playerListCount = 0;

    private void Awake()
    {
        ExitButton.onClick.AddListener(delegate
        {
            if(ServerInfo.sessionMode == SessionMode.HOST)
                MyNetworkManager.instance.StopHost();
            else if(ServerInfo.sessionMode == SessionMode.CLIENT)
                MyNetworkManager.instance.StopClient();
        });
        StartGameButton.onClick.AddListener(delegate
        {
            MyNetworkManager.instance.NetworkChangeScene("GameplayScene");
        });

        EventManager.instance.onPlayerListChanged += UpdateLobbyUI;

    }

    private void Update()
    {
        // Update only if the count changes
        if(playerListCount != MyNetworkManager.instance.roomSlots.Count)
        {
            UpdateLobbyUI();
        }

    }

    private void UpdateLobbyUI()
    {
        List<RoomPlayer> playerListCopy = new List<RoomPlayer>();
        foreach (var player in MyNetworkManager.instance.roomSlots)
        {
            if (player.TryGetComponent<RoomPlayer>(out RoomPlayer roomPlayer))
            {
                playerListCopy.Add(roomPlayer);
            }
        }

        if (RoomPlayer.Local != null)
        {
            // Make sure only host of the room can start the game
            if (RoomPlayer.Local.isServer)
                StartGameButton.interactable = true;
            else
                StartGameButton.interactable = false;

            Debug.Log($"Checking if local player is Host: {StartGameButton.interactable}");
        }

        // Toggle playerslot open or closed based on room max size
        for (int i = 0; i < 4; i++)
        {
            playerSlots[i].GetComponentInChildren<TMP_Text>().text = "Waiting for Player";
            if (i < playerListCopy.Count)
            {
                playerSlots[i].GetComponentInChildren<TMP_Text>().text = string.IsNullOrEmpty(playerListCopy[i].displayName) ? "Loading..." : playerListCopy[i].displayName;
            }
        }

        // Toggling closed
        for (int j = 4; j < ServerInfo.lobbyCap; j++)
        {
            playerSlots[j].GetComponentInChildren<TMP_Text>().text = "Unavailable";
        }
    }

    private void OnDestroy()
    {
        EventManager.instance.onPlayerListChanged -= UpdateLobbyUI;
    }
}
