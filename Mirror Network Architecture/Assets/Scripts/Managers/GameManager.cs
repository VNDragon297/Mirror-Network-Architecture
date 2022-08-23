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

    public GameObject SpawnWeapon(int index)
    {
        if (index >= MyNetworkManager.instance.spawnPrefabs.Count || index < 0)
            return null;

        var weapon = Instantiate(MyNetworkManager.instance.spawnPrefabs[index], Vector3.zero, Quaternion.identity);
        if(weapon.TryGetComponent<GunBehaviour>(out GunBehaviour gunBehaviour))
        {
            // This is to make sure that this is a weapon
            NetworkServer.Spawn(weapon);
            return weapon;
        }
        else
        {
            Destroy(weapon);
            return null;
        }
    }
}
