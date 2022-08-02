using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class MyNetworkManager : NetworkManager
{
    public static MyNetworkManager instance;

    [SerializeField] private Transport networkTransport;
    [SerializeField] private List<RoomPlayer> playerList;

    public override void Awake()
    {
        base.Awake();
        if (MyNetworkManager.instance != null)
            Destroy(this);
        else
            MyNetworkManager.instance = this;

        DontDestroyOnLoad(this);
    }

    public override void OnStartHost()
    {
        Debug.Log("Started ast Host");
        base.OnStartHost();
    }

    public override void OnStartServer()
    {
        Debug.Log("Started ast Server");
        base.OnStartServer();

        NetworkServer.RegisterHandler<InitializeMessage>(OnClientConnected);
    }

    public override void OnStartClient()
    {
        Debug.Log("Started ast Client");
        base.OnStartClient();
    }

    // Client only call
    public override void OnClientConnect()
    {
        base.OnClientConnect();

        Debug.LogWarning("Initiallize Room Player");
        InitializeMessage msg = new InitializeMessage
        {
            name = ClientInfo.DisplayName
        };

        NetworkClient.Send(msg);
    }

    // Client only call
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.LogWarning("Player disconnected");
    }

    private void OnClientConnected(NetworkConnectionToClient conn, InitializeMessage msg)
    {
        Debug.Log($"{msg.name} connected");

        var obj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        if(obj.TryGetComponent<RoomPlayer>(out RoomPlayer roomPlayer))
        {
            roomPlayer.playerName = msg.name;
            NetworkServer.Spawn(obj, conn);
            NetworkServer.AddPlayerForConnection(conn, obj);

            if (ServerInfo.sessionMode == SessionMode.SERVER || ServerInfo.sessionMode == SessionMode.HOST)
                playerList.Add(roomPlayer);

        }
        else
        {
            Debug.LogError("RoomPlayer script is missing from prefab");
            Debug.LogError("Please add script back to prefab and rebuild");
            Destroy(obj);
        }
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"Player disconnected");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // Remove client authority for all objects that are owned by client but not the base playerObj
        foreach (var obj in conn.clientOwnedObjects)
        {
            if (obj.TryGetComponent<RoomPlayer>(out RoomPlayer baseObj))
            {
                playerList.Remove(baseObj);
                Debug.Log($"Player {baseObj.playerName} disconnected");
            }
            else
                obj.RemoveClientAuthority();
        }
        NetworkServer.DestroyPlayerForConnection(conn);
    }
}
