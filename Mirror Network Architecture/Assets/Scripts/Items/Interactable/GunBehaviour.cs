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

                // Muzzle animation and draw raycast
                currentMagSize--;
                currentDelta = 0f;
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
        isReloading = false;
    }

    public void SetParentTransform(Transform parent) => this.SetParentTransform(parent);
}
