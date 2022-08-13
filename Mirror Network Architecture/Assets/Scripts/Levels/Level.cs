using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Level : NetworkBehaviour
{
    public static Level current { get; private set; }

    public Transform[] spawnPoints;

    private void Awake()
    {
        current = this;

        if (GameManager.instance == null)
            Debug.Log("Missing GameManager");

        if (Camera.main == null)
            Debug.Log("Missing Main Camera in scene");
    }
}
