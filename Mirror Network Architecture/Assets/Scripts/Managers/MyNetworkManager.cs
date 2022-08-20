using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class MyNetworkManager : NetworkRoomManager
{
    public static MyNetworkManager instance;

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
        // Debug.Log("Started ast Host");
        maxConnections = ServerInfo.lobbyMax <= 0 || ServerInfo.lobbyMax > ServerInfo.lobbyCap ? ServerInfo.lobbyCap : ServerInfo.lobbyMax;

        base.OnStartHost();
    }

    public override void OnStartServer()
    {
        // Debug.Log("Started ast Server");
        maxConnections = ServerInfo.lobbyMax <= 0 || ServerInfo.lobbyMax > ServerInfo.lobbyCap ? ServerInfo.lobbyCap : ServerInfo.lobbyMax;

        base.OnStartServer();

        NetworkServer.RegisterHandler<InitializeMessage>(OnClientConnected);
    }

    public override void OnStartClient()
    {
        // Debug.Log("Started ast Client");
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

        var gameManagerObj = Instantiate(spawnPrefabs[0], Vector3.zero, Quaternion.identity);
        var roomPlayerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        if(roomPlayerObj.TryGetComponent<RoomPlayer>(out RoomPlayer roomPlayer))
        {
            NetworkServer.Spawn(gameManagerObj, conn);
            NetworkServer.AddPlayerForConnection(conn, gameManagerObj);

            roomPlayer.displayName = msg.name;

            Debug.Log("Spawning Game Manager");
            NetworkServer.Spawn(roomPlayerObj, conn);
        }
        else
        {
            Debug.LogError("RoomPlayer script is missing from prefab");
            Debug.LogError("Please add script back to prefab and rebuild");
            Destroy(roomPlayerObj);
        }
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"Player connected (Server Side)");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // Remove client authority for all objects that are owned by client but not the base playerObj
        foreach (var obj in conn.clientOwnedObjects)
        {
            // Remove RoomPlayer and GameManager of the disconnected client
            if (obj.TryGetComponent<RoomPlayer>(out RoomPlayer clientRoomPlayer))
            {
                Debug.Log($"Player {clientRoomPlayer.displayName} disconnected");
                continue;
            }
            else if(obj.TryGetComponent<GameManager>(out GameManager clientGameManager))
            {
                continue;
            }
            else
                obj.RemoveClientAuthority();
        }
        NetworkServer.DestroyPlayerForConnection(conn);
    }

    public void LoadScene(string sceneName)
    {
        MyNetworkManager.instance.ServerChangeScene(sceneName);
    }

    [SerializeField] private List<NetworkStartPosition> startPositions;
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        // Clear spawn points upon new scene load
        startPositions.Clear();
        var coords = FindObjectsOfType<NetworkStartPosition>();
        foreach (var coord in coords)
            startPositions.Add(coord);

        // Spawn players
        if(string.Equals(sceneName, "Ascent"))
        {
            foreach(var player in roomSlots)
            {
                Debug.Log("Spawning players");

                // Determine spawn point
                Vector3 spawnPoint = (startPositions.Count > 0) ? startPositions[0].transform.position : new Vector3(0f, 1.5f, 0f);

                // If Spectators are allow, check here to know what to spawn
                var obj = Instantiate(spawnPrefabs[1], spawnPoint, Quaternion.identity);

                NetworkServer.Spawn(obj, player.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (MyNetworkManager.instance.isNetworkActive)
        {
            switch(ServerInfo.sessionMode)
            {
                case SessionMode.SERVER:
                    MyNetworkManager.instance.StopServer();
                    break;
                case SessionMode.HOST:
                    MyNetworkManager.instance.StopHost();
                    break;
                case SessionMode.CLIENT:
                    MyNetworkManager.instance.StopClient();
                    break;
            }
        }
    }
}
