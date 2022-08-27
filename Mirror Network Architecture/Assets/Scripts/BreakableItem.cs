using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BreakableItem : NetworkBehaviour
{
    public float maxHealth = 100f;
    [SyncVar(hook = nameof(OnTakenDamage))] public float currentHealth;

    [SerializeField] private GameObject MeshObject;

    private void Awake()
    {
        BuildObject();
    }

    private void BuildObject()
    {
        currentHealth = maxHealth;
        MeshObject.SetActive(true);
    }

    public void TakeDamage(float dmg)
    {
        CmdTakeDamage(dmg);
    }

    [Command(requiresAuthority = false)]
    private void CmdTakeDamage(float dmg)
    {
        Debug.Log($"Taken {dmg} damage");
        currentHealth -= dmg;
    }

    private void OnTakenDamage(float oldVal, float newVal)
    {
        Debug.Log($"Object is now at {newVal}");

        currentHealth = newVal;
        if(currentHealth <= 0f)
        {
            ObjectDestroyed();
        }
    }

    private void ObjectDestroyed()
    {
        // Play some particle shits here
        MeshObject.SetActive(false);
        Debug.Log($"{gameObject.name} destroyed");
        NetworkServer.UnSpawn(this.gameObject);
        Destroy(this.gameObject);
    }
}
