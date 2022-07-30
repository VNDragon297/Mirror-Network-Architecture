using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServerInfo
{
    public static string serverName;
    public static int maxPlayers;
    public static SessionMode sessionMode;
}

public enum SessionMode
{
    SERVER,
    CLIENT,
    HOST
}
