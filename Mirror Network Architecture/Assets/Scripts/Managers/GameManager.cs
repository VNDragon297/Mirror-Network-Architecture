using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public static Level currentLevel { get; private set; }

    private PlayerCameraController cameraController;
    public new Camera camera;

    public static bool IsCameraControlled => GameManager.instance.cameraController != null;
    public static void GetCameraControl(PlayerCameraController controller) => instance.cameraController = controller;

    public static void SetLevel(Level level) => currentLevel = level;

    public void SetCamera(Camera newCamera) => camera = newCamera;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        GameManager.instance = this;
    }

    private void LateUpdate()
    {
        if (cameraController == null) return;
        if (cameraController.Equals(null))
        {
            Debug.LogWarning("Ghost object for camera controller");
            cameraController = null;
            return;
        }

        if (cameraController.ControlCamera(camera) == false)
            cameraController = null;
    }
}
