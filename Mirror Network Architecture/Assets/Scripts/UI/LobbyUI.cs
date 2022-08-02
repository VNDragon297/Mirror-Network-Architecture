using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button ExitButton;

    private void Awake()
    {
        ExitButton.onClick.AddListener(delegate
        {
            if(ServerInfo.sessionMode == SessionMode.HOST)
                MyNetworkManager.instance.StopHost();
            else if(ServerInfo.sessionMode == SessionMode.CLIENT)
                MyNetworkManager.instance.StopClient();
        });
    }
}
