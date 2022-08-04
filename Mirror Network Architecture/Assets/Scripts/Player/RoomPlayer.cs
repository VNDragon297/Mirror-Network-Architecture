using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RoomPlayer : NetworkBehaviour
{
    public static RoomPlayer Local;

    [SyncVar(hook = nameof(OnNameChanged))] public string displayName;

    private void OnNameChanged(string oldName, string newName) => ClientInfo.DisplayName = newName;

    public override void OnStartAuthority()
    {
        Debug.Log($"Gained authority of {displayName}");
        base.OnStartAuthority();

        // Events when player gain control of this object;
    }

    private void Awake()
    {
    }

    private void Start()
    {
        if (isLocalPlayer)
            Local = this;

        Debug.Log($"Spawned in {displayName}");

        PlayerManager.playerList.Add(this);
        EventManager.instance.PlayerListChanged();
    }

    private void Update()
    {
        // Input authority check
        if (!isLocalPlayer)
            return;

    }

    private void OnDestroy()
    {
        PlayerManager.playerList.Remove(this);
        EventManager.instance.PlayerListChanged();
    }
}
