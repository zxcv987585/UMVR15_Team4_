using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadManager
{
    public enum Scene
    {
        TitleScene,
        LoadScene,
        Battle01,
        AN_Demo_Boss
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        LoadManager.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadScene.ToString());
    }

    public static void LoadCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
        if(targetScene == Scene.TitleScene) AudioManager.Instance.PlayBGM("BackGroundMusic");
    }
}
