using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct InitializeMessage : NetworkMessage
{
    public string name;
}
