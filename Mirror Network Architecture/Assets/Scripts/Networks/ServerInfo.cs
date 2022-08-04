using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServerInfo
{
    public static string serverName;
    public static int lobbyCap = 10;
    public static int lobbyMax;
    public static SessionMode sessionMode;
}

public enum SessionMode
{
    SERVER,
    CLIENT,
    HOST
}
