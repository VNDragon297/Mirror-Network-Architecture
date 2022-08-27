using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerEntity : PlayerComponent
{
    public PlayerController controller { get; private set; }
    public PlayerInput input { get; private set; }
    public PlayerAnimController animator;
    public int myIndex;

    private const int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int Kills;
    [SerializeField] private int Deaths;
    [SerializeField] private int Assists;
    public bool isDead => currentHealth <= 0;

    [Command(requiresAuthority = false)]
    public void CmdTakeDamage(int dmg, int index)
    {
        if(TryGetComponent<NetworkIdentity>(out NetworkIdentity netIdentity))
        {
            RpcTakeDamage(netIdentity.connectionToClient, dmg, index);
        }
    }

    [TargetRpc]
    private void RpcTakeDamage(NetworkConnection target, int dmg, int index)
    {
        currentHealth -= dmg;
        Debug.Log($"Taken damage from Player {index}");
        Debug.Log($"Current health: {currentHealth} (-{dmg})");
    }

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        input = GetComponent<PlayerInput>();
        myIndex = Players.IndexOf(this);

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
            SpawnPlayerInit();

            // Create HUD for local player
        }

        Players.Add(this);
    }

    private bool deathFlag = false;
    private void Update()
    {
        if (!hasAuthority)
            return;

        // Death flag to keep function from running multiple times
        if (isDead && deathFlag == false)
        {
            deathFlag = true;
            CmdOnPlayerDied();
        }
    }

    [Command]
    private void CmdOnPlayerDied()
    {
        RpcOnPlayerDied();
    }

    [ClientRpc(includeOwner = true)]
    private void RpcOnPlayerDied()
    {
        StartCoroutine(DeadCoroutine());
    }

    private IEnumerator DeadCoroutine()
    {
        Debug.Log("Player died");
        // Turn off collision and shits
        // Play dead animation here
        animator.SetAnimatorState(isDead, "isDead");
        yield return new WaitForSeconds(4f);
        StartCoroutine(WaitingToSpawn());
    }

    private IEnumerator WaitingToSpawn()
    {
        yield return new WaitForSeconds(4f);
        SpawnPlayerInit();
    }

    private void SpawnPlayerInit()
    {
        currentHealth = maxHealth;
        deathFlag = false;

        animator.SetAnimatorState(isDead, "isDead");
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
