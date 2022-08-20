using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    [SerializeField] private bool isAuto;
    [SerializeField] private bool isReloading;
    [SerializeField] private bool readyToFire;
    [SerializeField] private int maxMagSize;
    [SerializeField] private int currentMagSize;
    [SerializeField] private float reloadTime;
    [SerializeField] private float timeBetweenBullets;
    [SerializeField] private float currentDelta;

    [SerializeField] private GameObject muzzleFlash;

    private void Awake()
    {
        
    }

    private void FixedUpdate()
    {
        if (currentDelta < timeBetweenBullets)
            currentDelta += Time.fixedDeltaTime;
    }

    public void AttemptingToFire()
    {
        if (readyToFire && !isReloading)
            Firing();
    }

    public void ReReadyGunForFire() => readyToFire = true;

    private void Firing()
    {
        if (currentMagSize <= 0)
        {
            // Reload
            Reload();
            return;
        }

        if (isAuto)
        {
            if (currentDelta >= timeBetweenBullets)
            {

                // Fire bullet here
                currentMagSize--;
                currentDelta = 0f;
            }
        }
        else
        {
            if (currentDelta >= timeBetweenBullets)
            {

                // Fire bullet here
                currentMagSize--;
                currentDelta = 0f;
                readyToFire = false;
            }
        }
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
    }
}
