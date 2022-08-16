using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerCameraController : PlayerComponent
{
    public List<Transform> anchorPoints;                // Turn into list for multiple camera anchor
    public Transform currentPos;

    public float mouseSens = 1.0f;

    private void Awake()
    {
        if (currentPos == null)
            currentPos = anchorPoints[0];

        if (hasAuthority && !GameManager.IsCameraControlled)
            GameManager.GetCameraControl(this);
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        if (hasAuthority)
        {
            GameManager.GetCameraControl(this);
            GameManager.instance.SetCamera(Camera.main);
        }
    }

    public bool ControlCamera(Camera cam)
    {
        if (cam == null)
            return false;

        if(this.Equals(null))
        {
            Debug.LogWarning("Releasing camera from player");
            return false;
        }

        CameraPositionLerp(cam);
        return true;
    }

    private void CameraPositionLerp(Camera cam)
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, currentPos.position, Time.deltaTime * 100f);
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, currentPos.rotation, Time.deltaTime * 100f);
    }
}
