using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    private void Awake()
    {
        if (EventManager.instance != null && EventManager.instance != this)
            Destroy(this);
        else
            EventManager.instance = this;

        DontDestroyOnLoad(this);
    }

    public Action<string> onRoomPlayerSpawned;
    public void RoomPlayerSpawned(string name)
    {
        if (onRoomPlayerSpawned != null)
            onRoomPlayerSpawned(name);
    }
}
