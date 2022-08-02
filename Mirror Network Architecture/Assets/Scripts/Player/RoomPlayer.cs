using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RoomPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNameChanged))] public string playerName;

    private void OnNameChanged(string oldName, string newName)
    {
        ClientInfo.DisplayName = newName;
    }

    public override void OnStartAuthority()
    {
        Debug.Log($"Gained authority of {playerName}");
        base.OnStartAuthority();

        // Events when player gain control of this object;
    }

    private void Update()
    {
        // Input authority check
        if (!isLocalPlayer)
            return;
    }
}
