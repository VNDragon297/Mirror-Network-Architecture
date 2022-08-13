using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RoomPlayer : NetworkRoomPlayer
{
    public static RoomPlayer Local;

    [SyncVar(hook = nameof(OnNameChanged))] public string displayName;

    private void OnNameChanged(string oldName, string newName) => ClientInfo.DisplayName = newName;

    public override void OnStartAuthority()
    {
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
