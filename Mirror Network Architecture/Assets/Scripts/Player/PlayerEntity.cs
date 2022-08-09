using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : PlayerComponent
{
    public PlayerController controller { get; private set; }
    public PlayerInput input { get; private set; }

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        input = GetComponent<PlayerInput>();

        // Initialize all PlayerComponents in children objects
        var components = GetComponentsInChildren<PlayerComponent>();
        foreach(var component in components)
        {
            component.Init(this);
        }
    }

    public static readonly List<PlayerEntity> Players = new List<PlayerEntity>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(hasAuthority)
        {
            // Do what you need to do with local player here

            // Create HUD for local player
        }

        Players.Add(this);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        Players.Remove(this);
    }

    private void OnDestroy()
    {
        Players.Remove(this);
    }
}
