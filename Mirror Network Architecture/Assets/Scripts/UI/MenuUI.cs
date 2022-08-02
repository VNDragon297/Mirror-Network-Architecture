using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField DisplayNameInput;
    [SerializeField] private Button StartHostButton;
    [SerializeField] private Button StartServerButton;
    [SerializeField] private Button StartClientButton;
    private void OnEnable()
    {
        string displayName = PlayerPrefs.GetString("DisplayNameKey");
        DisplayNameInput.text = displayName;
        DisplayNameInput.ActivateInputField();

        StartHostButton.onClick.AddListener(delegate {
            ServerInfo.sessionMode = SessionMode.HOST;
            StartGame();
        });
        StartServerButton.onClick.AddListener(delegate {
            ServerInfo.sessionMode = SessionMode.SERVER;
            StartGame();
        });
        StartClientButton.onClick.AddListener(delegate {
            ServerInfo.sessionMode = SessionMode.CLIENT;
            StartGame();
        });
    }

    private void StartGame()
    {
        // Return if player does not have a name

        switch(ServerInfo.sessionMode)
        {
            case SessionMode.SERVER:
                MyNetworkManager.instance.StartServer();
                break;
            case SessionMode.HOST:
                if (string.IsNullOrEmpty(DisplayNameInput.text))
                    return;

                PlayerPrefs.SetString("DisplayNameKey", DisplayNameInput.text);
                ClientInfo.DisplayName = DisplayNameInput.text;

                MyNetworkManager.instance.StartHost();
                break;
            case SessionMode.CLIENT:
                if (string.IsNullOrEmpty(DisplayNameInput.text))
                    return;

                PlayerPrefs.SetString("DisplayNameKey", DisplayNameInput.text);
                ClientInfo.DisplayName = DisplayNameInput.text;

                MyNetworkManager.instance.StartClient();
                break;
            default:
                Debug.LogError("It should not get here");
                break;
        }
    }
}
