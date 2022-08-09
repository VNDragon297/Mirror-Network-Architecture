using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public enum SceneIndex
    {
        OfflineScene = 0,
        OnlineScene = 1,
        GameplayScene = 2
    }

    public static LevelManager instance;

    private void Awake()
    {
        if (LevelManager.instance != null && LevelManager.instance != this)
            Destroy(this);

        DontDestroyOnLoad(this);
    }

    public void LoadScene(SceneIndex scene)
    {
        if (SceneManager.GetActiveScene().buildIndex != (int)scene)
            return;

        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadNetworkScene(SceneIndex scene)
    {
        if (SceneManager.GetActiveScene().buildIndex != (int)scene)
            return;

        MyNetworkManager.instance.LoadScene(scene.ToString());
    }
}
