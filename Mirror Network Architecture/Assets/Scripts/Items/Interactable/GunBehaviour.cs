using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    public enum GunType
    {
        PRIMARY,
        SECONDARY
    }

    [SerializeField] private bool isAuto;
    [SerializeField] private bool isReloading;
    [SerializeField] private bool readyToFire;
    [SerializeField] private int maxMagSize;
    [SerializeField] private int currentMagSize;
    [SerializeField] private float reloadTime;
    [SerializeField] private float timeBetweenBullets;
    [SerializeField] private float currentDelta;
    [SerializeField] private GunType gunType;

    [SerializeField] private GameObject muzzleFlash;

    private void OnEnable()
    {
        currentMagSize = maxMagSize;
        currentDelta = 0;
    }

    private void FixedUpdate()
    {
        if (currentDelta < timeBetweenBullets)
            currentDelta += Time.fixedDeltaTime;
        else
            readyToFire = true;
    }

    public void AttemptingToFire(Vector3 startPos, Vector3 direction, float distance, int index)
    {
        if (readyToFire && !isReloading)
        {
            if (Shoot())
            {
                if (Physics.Raycast(startPos, direction, out RaycastHit hitInfo, distance))
                {
                    Debug.Log($"Raycast Hit! {hitInfo.collider.name}");
                    var playerEntityScript = hitInfo.collider.GetComponentInParent<PlayerEntity>();
                    if(playerEntityScript != null)
                    {
                        playerEntityScript.CmdTakeDamage(25, index);
                        return;
                    }

                    var itemScript = hitInfo.collider.GetComponentInParent<BreakableItem>();
                    if (itemScript != null)
                    {
                        itemScript.TakeDamage(25);
                        return;
                    }
                }
            }
        }
    }

    public void ReReadyGunForFire() => readyToFire = true;

    private bool Shoot()
    {
        if (currentMagSize <= 0)
        {
            // Reload
            Reload();
        }
        else
        {
            if (isAuto)
            {
                if (currentDelta >= timeBetweenBullets)
                {

                    // Muzzle animation and draw raycast
                    currentMagSize--;
                    currentDelta = 0f;
                    return true;
                }
            }
            else
            {
                if (currentDelta >= timeBetweenBullets)
                {

                    // Muzzle animation and draw raycast
                    currentMagSize--;
                    currentDelta = 0f;
                    readyToFire = false;
                    return true;
                }
            }
        }

        return false;
    }

    public void Reload()
    {
        // Start UI for reloading here if neccessary
        StartCoroutine(Reloading());
    }

    private IEnumerator Reloading()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentMagSize = maxMagSize;
        isReloading = false;
    }
}
