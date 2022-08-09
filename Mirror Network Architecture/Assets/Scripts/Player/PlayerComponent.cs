using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerComponent : NetworkBehaviour
{
    public PlayerEntity Entity { get; private set; }

    public virtual void Init(PlayerEntity entity)
    {
        Entity = entity;
    }

    public virtual void OnGameStart() { }
}
